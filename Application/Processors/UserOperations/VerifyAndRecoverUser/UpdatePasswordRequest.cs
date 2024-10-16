using System.ComponentModel.DataAnnotations;

namespace Application.Processors.UserOperations.VerifyAndRecoverUser;
public class UpdatePasswordRequest
{
    [Required]
    public string Password { get; set; }

    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; }
}

