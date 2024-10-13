using Microsoft.Extensions.DependencyInjection;
using Application.Processor;
using Infrastructure.Kafka;
using Domain.Broker;
using Application.Processor.UserOperations.CreateUser;

namespace Application.DependencyInjection;

public static class ApplicationExtension
{
    public static IServiceCollection AddProcessors(this IServiceCollection services)
    {
        services.AddScoped<IRequestProcessorFactory, RequestProcessorFactory>();
        services.AddScoped<IRequestProcessor, CreateUserProcessor>();
        services.AddScoped<CreateUserProcessor>();

        return services;    
    }

    public static IServiceCollection AddBrokerServices(this IServiceCollection services)
    {
        services.AddScoped<IConsumerService, KafkaConsumerService>();
        services.AddHostedService<KafkaConsumerHostedService>();

        return services;
    }
}