using System.ComponentModel.DataAnnotations;

namespace Application.Processors.GameOperations.AddRating;

public class AddRatingRequest
{
    public int GameId { get; set; }
    public Guid UserId { get; set; }
    public int RatingValue { get; set; }
    public string Review { get; set; }
}
