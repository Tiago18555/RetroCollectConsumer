using Domain;
using Domain.Broker;
using Domain.Entities;
using Domain.Repositories;
using CrossCutting;
using System.Text.Json;

namespace Application.Processors.UserWishlistOperations;

public class RemoveFromWishlistProcessor : IRequestProcessor
{
    private readonly IWishlistRepository _wishlistRepository;

    public RemoveFromWishlistProcessor(IWishlistRepository wishlistRepository)
    {
        _wishlistRepository = wishlistRepository;
    }

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<Wishlist>(field);
        var res = await RemoveAsync(request);

        return new MessageModel{ Message = res, SourceType = "remove-wishlist" };
    }

    public async Task<ResponseModel> RemoveAsync(Wishlist requestBody)
    {
        try
        {
            var result = await _wishlistRepository.DeleteAsync(
                new Wishlist { UserId = requestBody.UserId, GameId = requestBody.GameId }
            );

            if (result) return "Successfully Deleted".Ok();
            else return ResponseFactory.NotFound("Operation not successfully completed");
        }
        catch (Exception ex)
        {
            StdOut.Error(ex.Message);
            return ResponseFactory.ServiceUnavailable($"Error: {ex.Message}");
        }

    }
}
 
