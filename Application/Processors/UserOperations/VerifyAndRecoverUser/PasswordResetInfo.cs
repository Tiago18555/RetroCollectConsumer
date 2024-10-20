namespace Application.Processors.UserOperations.VerifyAndRecoverUser;
public class PasswordResetInfo
{
    public string Username { get; set; }
    public string TimeStampHash { get; set; }
    public bool Success { get; set; }
}


