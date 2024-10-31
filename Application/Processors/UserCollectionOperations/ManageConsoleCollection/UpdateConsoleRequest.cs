using System.Text.Json.Serialization;

namespace Application.Processors.UserCollectionOperations.ManageConsoleCollection;

public class UpdateConsoleRequest
{
    public Guid UserId { get; set; }
    public Guid UserConsoleId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Condition { get; set; }
    public string OwnershipStatus { get; set; }
    public string Notes { get; set; }
}
