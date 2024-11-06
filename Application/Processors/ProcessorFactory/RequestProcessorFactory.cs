using Domain.Broker;
using Microsoft.Extensions.DependencyInjection;
using Application.Processors.UserOperations.CreateUser;
using Application.Processors.UserOperations.ManageUser;
using Application.Processors.UserOperations.VerifyAndRecoverUser;
using Application.Processors.UserCollectionOperations.ManageGameCollection;
using Application.Processors.UserCollectionOperations.ManageConsoleCollection;
using Application.Processors.UserCollectionOperations.ManageComputerCollection;
using Application.Processors.UserWishlistOperations;
using Application.Processors.RatingOperations.AddRating;
using Application.Processors.RatingOperations.ManageRating;

namespace Application.Processors.ProcessorFactory;

public class RequestProcessorFactory : IRequestProcessorFactory
{
    private readonly IServiceProvider _sp;

    public RequestProcessorFactory(IServiceProvider serviceProvider)
    {
        _sp = serviceProvider;
    }

    public IRequestProcessor Create(string s, string topic)
    {
        if(String.IsNullOrWhiteSpace(topic))
            throw new ArgumentException($"invalid topic: {topic}", nameof(topic));

        IServiceScope scope = _sp.CreateScope();
        

        if(topic == "recover")
        {
            return s switch
            {
                "change-password" =>        scope.ServiceProvider.GetService<ChangePasswordProcessor>(),
                "recover-user" =>           scope.ServiceProvider.GetService<RecoverUserProcessor>(),
                "verify-user" =>            scope.ServiceProvider.GetService<VerifyUserProcessor>(),
                _ => throw new ArgumentException($"Unknown message type: {s}")
                
            };
        };

        if(topic == "collection")
        {
            return s switch
            {
                "add-game" =>               scope.ServiceProvider.GetService<AddGameCollectionProcessor>(),
                "delete-game" =>            scope.ServiceProvider.GetService<DeleteGameCollectionProcessor>(),
                "update-game" =>            scope.ServiceProvider.GetService<UpdateGameCollectionProcessor>(),

                "add-console" =>            scope.ServiceProvider.GetService<AddConsoleCollectionProcessor>(),
                "delete-console" =>         scope.ServiceProvider.GetService<DeleteConsoleCollectionProcessor>(),
                "update-console" =>         scope.ServiceProvider.GetService<UpdateConsoleCollectionProcessor>(),
                
                "add-computer" =>           scope.ServiceProvider.GetService<AddComputerCollectionProcessor>(),
                "delete-computer" =>        scope.ServiceProvider.GetService<DeleteComputerCollectionProcessor>(),
                "update-computer" =>        scope.ServiceProvider.GetService<UpdateComputerCollectionProcessor>(),
                _ => throw new ArgumentException($"Unknown message type: {s}")                
            };
        };

        if(topic == "user")
        {
            return s switch
            {
                "create-user" =>            scope.ServiceProvider.GetService<CreateUserProcessor>(),
                "update-user" =>            scope.ServiceProvider.GetService<ManageUserProcessor>(),
                _ => throw new ArgumentException($"Unknown message type: {s}")                
            };
        };

        if(topic == "rating")
        {
            return s switch
            {
                "add-rating" =>              scope.ServiceProvider.GetService<AddRatingProcessor>(),
                "edit-rating"=>              scope.ServiceProvider.GetService<EditRatingProcessor>(),
                "remove-rating" =>           scope.ServiceProvider.GetService<RemoveRatingProcessor>(),
                _ => throw new ArgumentException($"Unknown message type: {s}")
            };
        };

        if(topic == "wishlist")
        {
            return s switch
            {
                "remove-wishlist" =>        scope.ServiceProvider.GetService<RemoveFromWishlistProcessor>(),
                "add-wishlist" =>           scope.ServiceProvider.GetService<AddToWishlistProcessor>(),
                _ => throw new ArgumentException($"Unknown message type: {s}")
            };
        };

        throw new ArgumentException($"invalid topic: {topic}", nameof(topic));
    }
}

