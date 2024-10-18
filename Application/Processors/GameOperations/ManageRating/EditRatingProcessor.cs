using CrossCutting;
using CrossCutting.Providers;
using Domain;
using Domain.Broker;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace Application.Processors.GameOperations.ManageRating;

public class EditRatingProcessor : IRequestProcessor
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EditRatingProcessor(IRatingRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _ratingRepository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<EditRatingRequest>(field);
        var res = await EditRatingAsync(request);

        return new MessageModel{ Message = res, SourceType = "edit-rating" };
    }

    public async Task<ResponseModel> EditRatingAsync(EditRatingRequest requestBody)
    {
        try
        {
            var foundRating = await _ratingRepository.SingleOrDefaultAsync(x => x.RatingId == requestBody.RatingId);

            foundRating.RatingValue = requestBody.RatingValue == 0 ? foundRating.RatingValue : requestBody.RatingValue;
            foundRating.Review = String.IsNullOrEmpty(requestBody.Review) ? foundRating.Review : requestBody.Review;
            foundRating.UpdatedAt = _dateTimeProvider.UtcNow;

            var res = await _ratingRepository.UpdateAsync(foundRating);
            return
                res
                .MapObjectsTo(new EditRatingResponseModel())
                .Ok();
        }
        catch (DBConcurrencyException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
