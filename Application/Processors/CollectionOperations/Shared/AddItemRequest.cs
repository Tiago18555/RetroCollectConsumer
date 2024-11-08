using System.Text.Json.Serialization;

namespace Application.Processors.CollectionOperations.Shared;

public class AddItemRequest
{
    public Guid UserId { get; set; }
    public int ItemId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Condition { get; set; }
    public string OwnershipStatus { get; set; }
    public string Notes { get; set; }
}
