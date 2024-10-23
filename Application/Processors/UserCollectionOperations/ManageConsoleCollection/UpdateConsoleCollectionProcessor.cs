using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Console = Domain.Entities.Console;
using CrossCutting;
using Domain.Broker;
using Domain.Repositories;
using Application.IgdbIntegrationOperations.SearchConsole;
using System.Text.Json;
using Application.Processors.UserCollectionOperations.Shared;

namespace Application.Processors.UserCollectionOperations.ManageConsoleCollection;


public partial class UpdateConsoleCollectionProcessor : IRequestProcessor
{
    private readonly IConsoleRepository _consoleRepository;
    private readonly IUserConsoleRepository _userConsoleRepository;
    private readonly ISearchConsole _searchConsole;

    public UpdateConsoleCollectionProcessor (
        IConsoleRepository consoleRepository, 
        IUserConsoleRepository userConsoleRepository, 
        ISearchConsole searchConsole
    )
    {
        _consoleRepository = consoleRepository;
        _userConsoleRepository = userConsoleRepository;
        _searchConsole = searchConsole;
    }

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UpdateConsoleRequest>(field);
        var res = await UpdateConsoleAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "update-console" };
    }

    public async Task<ResponseModel> UpdateConsoleAsync(UpdateConsoleRequest request, CancellationToken cts)
    {
        try
        {
            var foundConsole = await _userConsoleRepository.SingleOrDefaultAsync(x => x.UserConsoleId == request.UserConsoleId, cts);
            if (! await _consoleRepository.AnyAsync(g => g.ConsoleId == foundConsole.ConsoleId, cts) && foundConsole.ConsoleId != 0)
            {
                var result = await _searchConsole.RetrieveConsoleInfoAsync(foundConsole.ConsoleId);

                var consoleInfo = result.Single();

                Console console = new()
                {
                    ConsoleId = consoleInfo.ConsoleId,
                    Description = consoleInfo.Description,
                    ImageUrl = consoleInfo.ImageUrl,
                    Name= consoleInfo.Name,
                    IsPortable= consoleInfo.IsPortable                        
                };
                await _consoleRepository.AddAsync(console, cts);

            }

            var newConsole = foundConsole.MapAndFill<UserConsole, UpdateConsoleRequest>(request);

            var res = await this._userConsoleRepository.UpdateAsync(newConsole, cts);

            return res.MapObjectsTo(new UpdateConsoleResponseModel()).Ok();
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
