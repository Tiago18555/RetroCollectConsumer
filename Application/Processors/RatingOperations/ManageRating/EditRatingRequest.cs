namespace Application.Processors.RatingOperations.ManageRating;

public class EditRatingRequest
{
    public Guid RatingId { get; set; }
    public int RatingValue { get; set; }
    public string Review { get; set; }
}
