using CrossCutting.Providers;
using Domain.Entities;

namespace Application.Processors.UserOperations.ManageUser;

public static class ManageUserHelper
{
    public static User MapAndFill(this User target, UpdateUserRequest source, DateTime utcnow)
    {
        if (source.Username != null) { target.Username = source.Username; }
        if (source.Email != null) { target.Email = source.Email; }
        if (source.FirstName != null) { target.FirstName = source.FirstName; }
        if (source.LastName != null) { target.LastName = source.LastName; }

        return target;
    }
}
