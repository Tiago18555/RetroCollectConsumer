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
using Application.Processors.CollectionOperations.Shared;

namespace Application.Processors.CollectionOperations.ManageComputerCollection;

public class UpdateComputerFromCollectionProcessor : IRequestProcessor
{
    private readonly IComputerRepository _computerRepository;
    private readonly IComputerCollectionItemRepository _userComputerRepository;
    private readonly ISearchComputer _searchComputer;

    public UpdateComputerFromCollectionProcessor (
        IComputerRepository computerRepository, 
        IComputerCollectionItemRepository userComputerRepository, 
        ISearchComputer searchComputer
    )
    {
        _computerRepository = computerRepository;
        _userComputerRepository = userComputerRepository;
        _searchComputer = searchComputer;
    }
    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();

        StdOut.Warning(field);

        var request = JsonSerializer.Deserialize<UpdateComputerRequest>(field);
        var res = await UpdateComputerAsync(request, cts);
    }

    private async Task<bool> UpdateComputerAsync(UpdateComputerRequest request, CancellationToken cts)
    {
        try
        {
            var foundComputer = await _userComputerRepository.SingleOrDefaultAsync(x => x.Id == request.Id, cts);

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

            var newComputer = foundComputer.MapAndFill<ComputerCollectionItem, UpdateComputerRequest>(request);

            var res = await this._userComputerRepository.UpdateAsync(newComputer, cts);

            StdOut.Info("computer updated");
            return true;
        }
        catch (ArgumentNullException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DBConcurrencyException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DbUpdateException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidOperationException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidClassTypeException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (NullClaimException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (Exception e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
    }
}
