using Domain;
using Application.Processors.UserCollectionOperations.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Console = Domain.Entities.Console;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using Application.IgdbIntegrationOperations.SearchConsole;
using System.Text.Json;

namespace Application.Processors.UserCollectionOperations.ManageConsoleCollection;

public partial class AddConsoleCollectionProcessor : IRequestProcessor
{
    private readonly IConsoleRepository _consoleRepository;
    private readonly IUserConsoleRepository _userConsoleRepository;
    private readonly ISearchConsole _searchConsole;

    public AddConsoleCollectionProcessor (
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
        var request = JsonSerializer.Deserialize<AddItemRequest>(field);
        var res = await AddConsoleAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "add-console" };
    }

    public async Task<ResponseModel> AddConsoleAsync(AddItemRequest requestBody, CancellationToken cts)
    {
        try
        {
            if (! await _consoleRepository.AnyAsync(g => g.ConsoleId == requestBody.ItemId, cts))
            {
                var result = await _searchConsole.RetrieveConsoleInfoAsync(requestBody.ItemId);

                var consoleInfo = result.Single();

                Console console = new()
                {
                    ConsoleId = consoleInfo.ConsoleId,
                    Description = consoleInfo.Description,
                    ImageUrl = consoleInfo.ImageUrl,
                    Name = consoleInfo.Name,
                    IsPortable = consoleInfo.IsPortable
                };

                await _consoleRepository.AddAsync(console, cts);
            }


            UserConsole userConsole = new()
            {
                ConsoleId = requestBody.ItemId,
                UserId = requestBody.UserId,
                Condition = Enum.Parse<Condition>(requestBody.Condition.ToCapitalize(typeof(Condition))),
                OwnershipStatus = Enum.Parse<OwnershipStatus>(requestBody.OwnershipStatus.ToCapitalize(typeof(OwnershipStatus))),
                Notes = requestBody.Notes == null ? null : requestBody.Notes,
                PurchaseDate = requestBody.PurchaseDate == DateTime.MinValue ? DateTime.MinValue : requestBody.PurchaseDate
            };

            var res = await _userConsoleRepository.AddAsync(userConsole, cts);
            return res.MapObjectsTo(new AddConsoleResponseModel()).Created();
        }
        
        catch (DBConcurrencyException)
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
        catch (InvalidEnumTypeException msg)
        {
            return ResponseFactory.UnsupportedMediaType("Invalid type for Condition or OwnershipStatus: " + msg);
        }
        catch (InvalidEnumValueException msg)
        {
            return ResponseFactory.BadRequest("Invalid value for Condition or OwnershipStatus: " + msg);
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
    }
}
