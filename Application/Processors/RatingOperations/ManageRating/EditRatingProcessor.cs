using CrossCutting;
using CrossCutting.Providers;
using Domain;
using Domain.Broker;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace Application.Processors.RatingOperations.ManageRating;

public class EditRatingProcessor : IRequestProcessor
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EditRatingProcessor(IRatingRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _ratingRepository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<EditRatingRequest>(field);
        var res = await EditRatingAsync(request, cts);
    }

    private async Task<bool> EditRatingAsync(EditRatingRequest requestBody, CancellationToken cts)
    {
        try
        {
            var foundRating = await _ratingRepository.SingleOrDefaultAsync(x => x.RatingId == requestBody.RatingId, cts);

            foundRating.RatingValue = requestBody.RatingValue == 0 ? foundRating.RatingValue : requestBody.RatingValue;
            foundRating.Review = String.IsNullOrEmpty(requestBody.Review) ? foundRating.Review : requestBody.Review;
            foundRating.UpdatedAt = _dateTimeProvider.UtcNow;

            var res = await _ratingRepository.UpdateAsync(foundRating, cts);
            StdOut.Info("rating updated");
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
        catch (Exception e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
    }
}
