using Domain.Broker;
using Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;

using Application.Processors.ProcessorFactory;
using Application.Processors.UserOperations.CreateUser;
using Application.Processors.UserOperations.ManageUser;
using Application.Processors.UserOperations.VerifyAndRecoverUser;
using Application.Processors.UserCollectionOperations.ManageComputerCollection;
using Application.Processors.UserCollectionOperations.ManageConsoleCollection;
using Application.Processors.UserCollectionOperations.ManageGameCollection;
using Application.Processors.UserWishlistOperations;
using Application.IgdbIntegrationOperations.SearchComputer;
using Application.IgdbIntegrationOperations.SearchConsole;
using Application.IgdbIntegrationOperations.SearchGame;
using Infrastructure.Kafka.Consumers;
using Application.Processors.RatingOperations.AddRating;
using Application.Processors.RatingOperations.ManageRating;

namespace Application.DependencyInjection;

public static class ApplicationExtension
{
    public static IServiceCollection AddProcessors(this IServiceCollection services)
    {
        services.AddSingleton<IRequestProcessorFactory, RequestProcessorFactory>();

        /** USER OPERATIONS **/
        services.AddScoped<CreateUserProcessor>();
        services.AddScoped<ManageUserProcessor>();
        services.AddScoped<RecoverUserProcessor>();
        services.AddScoped<ChangePasswordProcessor>();
        services.AddScoped<VerifyUserProcessor>();

        /** COLLECTION OPERATIONS **/
        services.AddScoped<AddComputerCollectionProcessor>();
        services.AddScoped<DeleteComputerCollectionProcessor>();
        services.AddScoped<UpdateComputerCollectionProcessor>();
        services.AddScoped<AddConsoleCollectionProcessor>();
        services.AddScoped<DeleteConsoleCollectionProcessor>();
        services.AddScoped<UpdateConsoleCollectionProcessor>();
        services.AddScoped<AddGameCollectionProcessor>();
        services.AddScoped<DeleteGameCollectionProcessor>();
        services.AddScoped<UpdateGameCollectionProcessor>();

        /** USER WISHLIST OPERATIONS **/
        services.AddScoped<AddToWishlistProcessor>();
        services.AddScoped<RemoveFromWishlistProcessor>();

        /** RATING OPERATIONS **/
        services.AddScoped<AddRatingProcessor>();
        services.AddScoped<RemoveRatingProcessor>();
        services.AddScoped<EditRatingProcessor>();

        return services;    
    }

    public static IServiceCollection AddBrokerServices(this IServiceCollection services)
    {
        services.AddSingleton<CollectionConsumerService>();
        services.AddSingleton<RatingConsumerService>();
        services.AddSingleton<RecoverConsumerService>();
        services.AddSingleton<UserConsumerService>();
        services.AddSingleton<WishlistConsumerService>();

        services.AddHostedService<KafkaConsumerHostedService>();

        return services;
    }

    public static IServiceCollection AddIgdbServices(this IServiceCollection services)
    {
        services.AddScoped<ISearchComputer, SearchComputer>();
        services.AddScoped<ISearchConsole, SearchConsole>();
        services.AddScoped<ISearchGame, SearchGame>();

        return services;
    }
}