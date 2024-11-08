using Application.Processors.CollectionOperations.Shared;
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

namespace Application.Processors.CollectionOperations.ManageConsoleCollection;

public partial class AddConsoleCollectionProcessor : IRequestProcessor
{
    private readonly IConsoleRepository _consoleRepository;
    private readonly IConsoleCollectionItemRepository _userConsoleRepository;
    private readonly ISearchConsole _searchConsole;

    public AddConsoleCollectionProcessor (
        IConsoleRepository consoleRepository, 
        IConsoleCollectionItemRepository userConsoleRepository, 
        ISearchConsole searchConsole
    )
    {
        _consoleRepository = consoleRepository;
        _userConsoleRepository = userConsoleRepository;
        _searchConsole = searchConsole;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<AddItemRequest>(field);
        var res = await AddConsoleAsync(request, cts);
    }

    private async Task<bool> AddConsoleAsync(AddItemRequest requestBody, CancellationToken cts)
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


            ConsoleCollectionItem userConsole = new()
            {
                ConsoleId = requestBody.ItemId,
                UserId = requestBody.UserId,
                Condition = Enum.Parse<Condition>(requestBody.Condition.ToCapitalize(typeof(Condition))),
                OwnershipStatus = Enum.Parse<OwnershipStatus>(requestBody.OwnershipStatus.ToCapitalize(typeof(OwnershipStatus))),
                Notes = requestBody.Notes == null ? null : requestBody.Notes,
                PurchaseDate = requestBody.PurchaseDate == DateTime.MinValue ? DateTime.MinValue : requestBody.PurchaseDate
            };

            var res = await _userConsoleRepository.AddAsync(userConsole, cts);
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
        catch (InvalidEnumTypeException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidEnumValueException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (NullClaimException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
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
    }
}
