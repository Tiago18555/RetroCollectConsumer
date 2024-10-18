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

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserConsole>(field);
        var res = await UpdateConsoleAsync(request);

        return new MessageModel{ Message = res, SourceType = "update-console" };
    }

    public async Task<ResponseModel> UpdateConsoleAsync(UserConsole newConsole)
    {
        try
        {
            if (!_consoleRepository.Any(g => g.ConsoleId == newConsole.ConsoleId) && newConsole.ConsoleId != 0)
            {
                var result = await _searchConsole.RetrieveConsoleInfoAsync(newConsole.ConsoleId);

                var consoleInfo = result.Single();

                Console console = new()
                {
                    ConsoleId = consoleInfo.ConsoleId,
                    Description = consoleInfo.Description,
                    ImageUrl = consoleInfo.ImageUrl,
                    Name= consoleInfo.Name,
                    IsPortable= consoleInfo.IsPortable                        
                };
                _consoleRepository.Add(console);

            }

            var res = this._userConsoleRepository.Update(newConsole);

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
