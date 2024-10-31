using System.Text.Json.Serialization;

namespace Application.Processors.UserCollectionOperations.ManageGameCollection;

public class UpdateGameRequest
{
    public Guid UserId { get; set; }
    public Guid UserCollectionId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Condition { get; set; }
    public string OwnershipStatus { get; set; }
    public string Notes { get; set; }
}