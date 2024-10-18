using Domain.Entities;

namespace Application.Processors.UserWishlistOperations;

public class WishlistResponseModel
{
    public Guid WishlistId { get; set; }
    private Game Game { get; set; }
    public string game => Game.Title ?? "";
}
 
