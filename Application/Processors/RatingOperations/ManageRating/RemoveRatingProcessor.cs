using CrossCutting;
using Domain;
using Domain.Broker;
using Domain.Entities;
using Domain.Repositories;
using System.Text.Json;

namespace Application.Processors.RatingOperations.ManageRating;

public class RemoveRatingProcessor : IRequestProcessor
{
    private readonly IRatingRepository _ratingRepository;

    public RemoveRatingProcessor(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<Rating>(field);
        var res = await RemoveRatingAsync(request, cts);
    }
    private async Task<bool> RemoveRatingAsync(Rating requestBody, CancellationToken cts)
    {
        try
        {
            var success = await _ratingRepository.DeleteAsync(requestBody, cts);

            if (success)
            {
                StdOut.Info("Rating Deleted");
                return true;
            }

            StdOut.Error($"Not deleted");
            return false;
        }
        catch (ArgumentNullException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidOperationException e)
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
