using Domain;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using Application.IgdbIntegrationOperations.SearchComputer;
using System.Text.Json;
using Application.Processors.UserCollectionOperations.Shared;

namespace Application.Processors.UserCollectionOperations.ManageComputerCollection;

public class AddComputerCollectionProcessor : IRequestProcessor
{
    private readonly IComputerRepository _computerRepository;
    private readonly IUserComputerRepository _userComputerRepository;
    private readonly ISearchComputer _searchComputer;
    public AddComputerCollectionProcessor(
        IComputerRepository computerRepository,
        IUserComputerRepository userComputerRepository,
        ISearchComputer searchComputer
    )
    {
        this._computerRepository = computerRepository;
        this._userComputerRepository = userComputerRepository;
        this._searchComputer = searchComputer;
    }
    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<AddItemRequestModel>(field);
        var res = await AddComputerAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "add-computer" };
    }

    public async Task<ResponseModel> AddComputerAsync(AddItemRequestModel requestBody, CancellationToken cts)
    {
        if (!await _computerRepository.AnyAsync(g => g.ComputerId == requestBody.Item_id, cts))
        {
            try
            {
                var result = await _searchComputer.RetrieveComputerInfoAsync(requestBody.Item_id);

                var computerInfo = result.Single();

                Computer computer = new()
                {
                    ComputerId = computerInfo.ComputerId,
                    Description = computerInfo.Description,
                    ImageUrl = computerInfo.ImageUrl,
                    Name = computerInfo.Name,
                    IsArcade = computerInfo.IsArcade
                };


                var res = await _computerRepository.AddAsync(computer, cts);
            }
            catch (NullClaimException msg)
            {
                return ResponseFactory.BadRequest(msg.ToString());
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }


        try
        {
            UserComputer userComputer = new()
            {
                ComputerId = requestBody.Item_id,
                UserId = requestBody.User_id,
                Condition = Enum.Parse<Condition>(requestBody.Condition.ToCapitalize(typeof(Condition))),
                OwnershipStatus = Enum.Parse<OwnershipStatus>(requestBody.OwnershipStatus.ToCapitalize(typeof(OwnershipStatus))),
                Notes = requestBody.Notes == null ? null : requestBody.Notes,
                PurchaseDate = requestBody.PurchaseDate == DateTime.MinValue ? DateTime.MinValue : requestBody.PurchaseDate
            };

            var res = await _userComputerRepository.AddAsync(userComputer, cts);
            return res.MapObjectsTo(new AddComputerResponseModel()).Created();
        }
        catch (DBConcurrencyException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (InvalidEnumTypeException msg)
        {
            return ResponseFactory.UnsupportedMediaType("Invalid type for Condition or OwnershipStatus: " + msg);
        }
        catch (InvalidEnumValueException msg)
        {
            return ResponseFactory.BadRequest("Invalid value for Condition or OwnershipStatus: " + msg);
        }
    }
}
