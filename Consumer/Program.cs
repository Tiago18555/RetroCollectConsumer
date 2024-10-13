using Application.DependencyInjection;
using CrossCutting.Providers;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("Local");
        
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUserCollectionRepository, UserCollectionRepository>();
        services.AddScoped<IUserComputerRepository, UserComputerRepository>();
        services.AddScoped<IUserConsoleRepository, UserConsoleRepository>();

        services.AddScoped<IConsoleRepository, ConsoleRepository>();
        services.AddScoped<IComputerRepository, ComputerRepository>();
        services.AddScoped<IGameRepository, GameRepository>();

        services.AddScoped<IRecoverRepository, MongoRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();

        services.AddProcessors();
        services.AddBrokerServices();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IMongoDatabase>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetSection("MongoDB")["ConnectionString"];
            var databaseName = configuration.GetSection("MongoDatabaseName")["DatabaseName"];
            var client = new MongoClient(connectionString);
            return client.GetDatabase(databaseName);
        });
        services.AddScoped<MongoDBContext>();
        services.AddDbContext<DataContext>(o =>
        {
            o.UseNpgsql(connectionString);
            o.EnableSensitiveDataLogging();
        });
    })
    .Build();

await host.RunAsync();