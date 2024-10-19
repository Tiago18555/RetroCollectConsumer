using Domain;
using CrossCutting;
using Domain.Broker;
using Domain.Entities;
using Domain.Repositories;
using System.Text.Json;

namespace Application.Processors.UserWishlistOperations;

public class AddToWishlistProcessor : IRequestProcessor
{
    private readonly IWishlistRepository _wishlistRepository;

    public AddToWishlistProcessor(IWishlistRepository repository)
    {
        _wishlistRepository = repository;
    }

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<Wishlist>(field);
        var res = await AddAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "add-wishlist" };
    }

    public async Task<ResponseModel> AddAsync(Wishlist requestBody, CancellationToken cts)
    {
        try
        {
            var wishlist = new Wishlist { UserId = requestBody.UserId, GameId = requestBody.GameId };
            var res = await _wishlistRepository.AddAsync(wishlist, cts);

            return res.MapObjectTo(new AddToUserWishlistResponseModel()).Ok();
        }
        catch (Exception ex)
        {
            StdOut.Error(ex.Message);
            return ResponseFactory.ServiceUnavailable($"Error: {ex.Message}");
        }
    }
}
 
