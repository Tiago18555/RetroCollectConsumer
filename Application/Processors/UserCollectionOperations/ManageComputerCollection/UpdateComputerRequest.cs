namespace Application.Processors.UserCollectionOperations.ManageComputerCollection;

public class UpdateComputerRequest
{
    public Guid UserId { get; set; }

    public Guid UserComputerId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Condition { get; set; }
    public string OwnershipStatus { get; set; }
    public string Notes { get; set; }
}
