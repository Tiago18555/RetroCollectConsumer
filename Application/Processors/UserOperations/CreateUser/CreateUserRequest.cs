using System.Text.Json.Serialization;

namespace Application.Processors.UserOperations.CreateUser;

public class CreateUserRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
