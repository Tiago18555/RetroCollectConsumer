using System.Text.Json.Serialization;

namespace Application.Processors.UserOperations.VerifyAndRecoverUser;
public class UpdatePasswordRequest
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

