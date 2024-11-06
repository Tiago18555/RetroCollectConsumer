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

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<Wishlist>(field);
        var res = await RemoveAsync(request, cts);
    }

    private async Task<bool> RemoveAsync(Wishlist requestBody, CancellationToken cts)
    {
        try
        {
            var found = await _wishlistRepository.SingleOrDefaultAsync(x => 
                x.UserId == requestBody.UserId && 
                x.GameId == requestBody.GameId, cts
            );

            var result = await _wishlistRepository.DeleteAsync(found, cts);

            if (result) 
            {
                StdOut.Info("removed from wishlist");
                return true;
            }
            else 
            {
                StdOut.Error("not removed from wishlist");
                return false;
            }
        }
        catch (Exception ex)
        {
            StdOut.Error(ex.Message);
            return false;
        }

    }
}
 
