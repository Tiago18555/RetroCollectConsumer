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
    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<AddItemRequest>(field);
        var res = await AddComputerAsync(request, cts);
    }

    private async Task<bool> AddComputerAsync(AddItemRequest requestBody, CancellationToken cts)
    {
        try
        {
            if (!await _computerRepository.AnyAsync(g => g.ComputerId == requestBody.ItemId, cts))
            {
                var result = await _searchComputer.RetrieveComputerInfoAsync(requestBody.ItemId);

                var computerInfo = result.Single();

                Computer computer = new()
                {
                    ComputerId = computerInfo.ComputerId,
                    Description = computerInfo.Description,
                    ImageUrl = computerInfo.ImageUrl,
                    Name = computerInfo.Name,
                    IsArcade = computerInfo.IsArcade
                };


                await _computerRepository.AddAsync(computer, cts);
            }

            UserComputer userComputer = new()
            {
                ComputerId = requestBody.ItemId,
                UserId = requestBody.UserId,
                Condition = Enum.Parse<Condition>(requestBody.Condition.ToCapitalize(typeof(Condition))),
                OwnershipStatus = Enum.Parse<OwnershipStatus>(requestBody.OwnershipStatus.ToCapitalize(typeof(OwnershipStatus))),
                Notes = requestBody.Notes == null ? null : requestBody.Notes,
                PurchaseDate = requestBody.PurchaseDate == DateTime.MinValue ? DateTime.MinValue : requestBody.PurchaseDate
            };

            var res = await _userComputerRepository.AddAsync(userComputer, cts);
            StdOut.Info("computer add to collection");
            return true;
        }
        catch (DBConcurrencyException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DbUpdateConcurrencyException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DbUpdateException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (NullClaimException e)
        {
            StdOut.Error($"null claim: {e.Message}");
            return false;
        }
        catch (InvalidOperationException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (ArgumentNullException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidEnumTypeException e)
        {
            StdOut.Error($"Invalid type for Condition or OwnershipStatus: {e.Message}");
            return false;
        }
        catch (InvalidEnumValueException e)
        {
            StdOut.Error($"Invalid value for Condition or OwnershipStatus: {e.Message}");
            return false;
        }
    }
}
