using System.Text.Json.Serialization;

namespace Application.Processors.CollectionOperations.ManageGameCollection;

public class AddGameRequest
{
    public Guid UserId { get; set; }
    public int GameId { get; set; }
    public int PlatformId { get; set; }
    public bool PlatformIsComputer { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Condition { get; set; }
    public string OwnershipStatus { get; set; }
    public string Notes { get; set; }
}