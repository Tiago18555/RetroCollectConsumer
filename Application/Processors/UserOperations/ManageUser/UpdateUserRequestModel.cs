using System.ComponentModel.DataAnnotations;

namespace Application.Processors.UserOperations.ManageUser;

public class UpdateUserRequest
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

}
