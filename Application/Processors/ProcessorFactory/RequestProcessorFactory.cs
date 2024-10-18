using Domain.Broker;
using Microsoft.Extensions.DependencyInjection;
using Application.Processors.UserOperations.CreateUser;
using Application.Processors.UserOperations.ManageUser;
using Application.Processors.UserOperations.VerifyAndRecoverUser;
using Application.Processors.UserCollectionOperations.ManageGameCollection;
using Application.Processors.UserCollectionOperations.ManageConsoleCollection;
using Application.Processors.UserCollectionOperations.ManageComputerCollection;

namespace Application.Processors.ProcessorFactory;

public class RequestProcessorFactory : IRequestProcessorFactory
{
    private readonly IServiceProvider _sp;

    public RequestProcessorFactory(IServiceProvider serviceProvider)
    {
        _sp = serviceProvider;
    }

    public IRequestProcessor Create(string s)
    {
        return s switch
        {
            /** USER OPERATIONS **/
            "create-user" =>            _sp.GetService<CreateUserProcessor>(),
            "update-user" =>            _sp.GetService<ManageUserProcessor>(),
            "change-password" =>        _sp.GetService<ChangePasswordProcessor>(),
            "verify-recover-user" =>    _sp.GetService<VerifyAndRecoverUserProcessor>(),


            /** GAME OPERATIONS **/
            "add-game" =>               _sp.GetService<AddGameCollectionProcessor>(),
            "delete-game" =>            _sp.GetService<DeleteGameCollectionProcessor>(),
            "update-game" =>            _sp.GetService<UpdateGameCollectionProcessor>(),

            "add-console" =>            _sp.GetService<AddConsoleCollectionProcessor>(),
            "delete-console" =>         _sp.GetService<DeleteConsoleCollectionProcessor>(),
            "update-console" =>         _sp.GetService<UpdateConsoleCollectionProcessor>(),
            
            "add-computer" =>           _sp.GetService<AddComputerCollectionProcessor>(),
            "delete-computer" =>        _sp.GetService<DeleteComputerCollectionProcessor>(),
            "update-computer" =>        _sp.GetService<UpdateComputerCollectionProcessor>(),

            /** USER WISHLIST OPERATIONS **/

            _ => throw new ArgumentException($"Unknown message type: {s}")
        };
    }
}

