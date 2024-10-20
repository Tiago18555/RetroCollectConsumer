using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using Application.IgdbIntegrationOperations.SearchComputer;
using System.Text.Json;

namespace Application.Processors.UserCollectionOperations.ManageComputerCollection;

public class UpdateComputerCollectionProcessor : IRequestProcessor
{
    private readonly IComputerRepository _computerRepository;
    private readonly IUserComputerRepository _userComputerRepository;
    private readonly ISearchComputer _searchComputer;

    public UpdateComputerCollectionProcessor (
        IComputerRepository computerRepository, 
        IUserComputerRepository userComputerRepository, 
        ISearchComputer searchComputer
    )
    {
        _computerRepository = computerRepository;
        _userComputerRepository = userComputerRepository;
        _searchComputer = searchComputer;
    }
        public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserComputer>(field);
        var res = await UpdateComputerAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "update-computer" };
    }

    public async Task<ResponseModel> UpdateComputerAsync(UserComputer newComputer, CancellationToken cts)
    {
        try
        {
            if (! await _computerRepository.AnyAsync(g => g.ComputerId == newComputer.ComputerId, cts) && newComputer.ComputerId != 0)
            {
                var result = await _searchComputer.RetrieveComputerInfoAsync(newComputer.ComputerId);

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

            var res = await this._userComputerRepository.UpdateAsync(newComputer, cts);

            return res.MapObjectsTo(new UpdateComputerResponseModel()).Ok();
        }
        catch (ArgumentNullException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido");
        }
        catch (DBConcurrencyException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido");
        }
        catch (DbUpdateException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido");
        }
        catch (InvalidOperationException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido.");
        }
        catch (InvalidClassTypeException msg)
        {
            //throw;
            return ResponseFactory.ServiceUnavailable(msg.ToString());
        }
        catch (NullClaimException msg)
        {
            return ResponseFactory.BadRequest(msg.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }
}
