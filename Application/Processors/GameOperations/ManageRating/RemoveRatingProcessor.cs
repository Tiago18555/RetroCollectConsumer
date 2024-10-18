using CrossCutting;
using Domain;
using Domain.Broker;
using Domain.Entities;
using Domain.Repositories;
using System.Text.Json;

namespace Application.Processors.GameOperations.ManageRating;

public class RemoveRatingProcessor : IRequestProcessor
{
    private readonly IRatingRepository _ratingRepository;

    public RemoveRatingProcessor(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<Rating>(field);
        var res = await RemoveRatingAsync(request);

        return new MessageModel{ Message = res, SourceType = "remove-rating" };
    }
    public async Task<ResponseModel> RemoveRatingAsync(Rating requestBody)
    {
        try
        {
            var success = await _ratingRepository.DeleteAsync(requestBody);

            if (success)
            {
                StdOut.Info("Rating Deleted");
                return "Rating Deleted".Ok();                
            }

            StdOut.Error($"Not deleted");
            return ResponseFactory.ServiceUnavailable($"Rating Deleted");
        }
        catch (ArgumentNullException e)
        {
            return ResponseFactory.ServiceUnavailable(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return ResponseFactory.ServiceUnavailable(e.Message);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
