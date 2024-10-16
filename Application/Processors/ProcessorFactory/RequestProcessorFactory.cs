using Domain.Broker;
using Microsoft.Extensions.DependencyInjection;
using Application.Processors.UserOperations.CreateUser;
using Application.Processors.UserOperations.ManageUser;
using Application.Processors.UserOperations.VerifyAndRecoverUser;

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

            /** USER WISHLIST OPERATIONS **/

            _ => throw new ArgumentException($"Unknown message type: {s}")
        };
    }
}

