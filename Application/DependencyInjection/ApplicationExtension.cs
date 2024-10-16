using Domain.Broker;
using Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;

using Application.Processors.UserOperations.CreateUser;
using Application.Processors.UserOperations.ManageUser;
using Application.Processors.UserOperations.VerifyAndRecoverUser;
using Application.Processors.ProcessorFactory;

namespace Application.DependencyInjection;

public static class ApplicationExtension
{
    public static IServiceCollection AddProcessors(this IServiceCollection services)
    {
        services.AddScoped<IRequestProcessorFactory, RequestProcessorFactory>();

        /** USER OPERATIONS **/
        services.AddScoped<CreateUserProcessor>();
        services.AddScoped<ManageUserProcessor>();
        services.AddScoped<VerifyAndRecoverUserProcessor>();
        services.AddScoped<ChangePasswordProcessor>();

        /** GAME OPERATIONS **/

        /** USER WISHLIST OPERATIONS **/

        return services;    
    }

    public static IServiceCollection AddBrokerServices(this IServiceCollection services)
    {
        services.AddScoped<IConsumerService, KafkaConsumerService>();
        services.AddHostedService<KafkaConsumerHostedService>();

        return services;
    }
}