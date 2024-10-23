using Application.Processors.UserCollectionOperations.ManageComputerCollection;
using Application.Processors.UserCollectionOperations.ManageConsoleCollection;
using Application.Processors.UserCollectionOperations.ManageGameCollection;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Processors.UserCollectionOperations.Shared;

public static class ManageUserCollectionHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static TTarget MapAndFill<TTarget, TSource>(this object target, TSource source) where TTarget : class //target banco / source request
    {
        if (typeof(TSource) == typeof(UpdateGameRequest) && typeof(TTarget) == typeof(UserCollection))
        {
            var userCollection = target as UserCollection;
            var request = source as UpdateGameRequest;

            if (request.PurchaseDate != DateTime.MinValue) { userCollection.PurchaseDate = request.PurchaseDate; }

            if (request.UserId != default) { userCollection.UserId = request.UserId; }

            if (!string.IsNullOrEmpty(request.Notes)) { userCollection.Notes = request.Notes; }
            if (!string.IsNullOrEmpty(request.Condition)) { userCollection.Condition = Enum.Parse<Condition>(request.Condition); }
            if (!string.IsNullOrEmpty(request.OwnershipStatus)) { userCollection.OwnershipStatus = Enum.Parse<OwnershipStatus>(request.OwnershipStatus); }

            return userCollection as TTarget;

        }
        if (typeof(TSource) == typeof(UpdateComputerRequest) && typeof(TTarget) == typeof(UserComputer))
        {
            var userComputer = target as UserComputer;
            var computer = source as UpdateComputerRequest;

            if (computer.PurchaseDate != DateTime.MinValue) { userComputer.PurchaseDate = computer.PurchaseDate; }

            if (computer.UserId != default) { userComputer.UserId = computer.UserId; }

            if (!string.IsNullOrEmpty(computer.Notes)) { userComputer.Notes = computer.Notes; }
            if (!string.IsNullOrEmpty(computer.Condition)) { userComputer.Condition = Enum.Parse<Condition>(computer.Condition); }
            if (!string.IsNullOrEmpty(computer.OwnershipStatus)) { userComputer.OwnershipStatus = Enum.Parse<OwnershipStatus>(computer.OwnershipStatus); } 

            return userComputer as TTarget;
        }
        if (typeof(TSource) == typeof(UpdateConsoleRequest) && typeof(TTarget) == typeof(UserConsole))
        {
            var userConsole = target as UserConsole;
            var console = source as UpdateConsoleRequest;

            if (console.PurchaseDate != DateTime.MinValue) { userConsole.PurchaseDate = console.PurchaseDate; }

            if (console.UserId != default) { userConsole.UserId = console.UserId; }

            if (!string.IsNullOrEmpty(console.Notes)) { userConsole.Notes = console.Notes; }
            if (!string.IsNullOrEmpty(console.Condition)) { userConsole.Condition = Enum.Parse<Condition>(console.Condition); }
            if (!string.IsNullOrEmpty(console.OwnershipStatus)) { userConsole.OwnershipStatus = Enum.Parse<OwnershipStatus>(console.OwnershipStatus); }

            return userConsole as TTarget;
        }
        else
        {
            throw new InvalidClassTypeException($"Invalid Type: {nameof(source)}, or {nameof(target)}");
        }
    }
}
