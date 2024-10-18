using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Processors.UserWishlistOperations;

public class AddToUserWishlistRequest
{
    public int Id { get; set; }
}
