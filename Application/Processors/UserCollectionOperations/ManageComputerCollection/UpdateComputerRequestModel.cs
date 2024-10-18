using CrossCutting.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Processors.UserCollectionOperations.ManageComputerCollection;

public class UpdateComputerRequestModel
{
    public Guid User_id { get; set; }
    public int Item_id { get; set; }
    public Guid UserComputerId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Condition { get; set; }
    public string OwnershipStatus { get; set; }
    public string Notes { get; set; }
}
