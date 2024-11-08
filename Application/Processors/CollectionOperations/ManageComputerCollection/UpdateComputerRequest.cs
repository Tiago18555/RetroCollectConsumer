using System.Text.Json.Serialization;

namespace Application.Processors.CollectionOperations.ManageComputerCollection;

public class UpdateComputerRequest
{
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Condition { get; set; }
    public string OwnershipStatus { get; set; }
    public string Notes { get; set; }
}
