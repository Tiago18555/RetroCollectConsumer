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
using Application.Processors.UserCollectionOperations.Shared;

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

        StdOut.Warning(field);

        var request = JsonSerializer.Deserialize<UpdateComputerRequest>(field);
        var res = await UpdateComputerAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "update-computer" };
    }

    public async Task<ResponseModel> UpdateComputerAsync(UpdateComputerRequest request, CancellationToken cts)
    {
        try
        {
            var foundComputer = await _userComputerRepository.SingleOrDefaultAsync(x => x.UserComputerId == request.UserComputerId, cts);

            if (! await _computerRepository.AnyAsync(g => g.ComputerId == foundComputer.ComputerId, cts) && foundComputer.ComputerId != 0)
            {
                var result = await _searchComputer.RetrieveComputerInfoAsync(foundComputer.ComputerId);

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

            var newComputer = foundComputer.MapAndFill<UserComputer, UpdateComputerRequest>(request);

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
