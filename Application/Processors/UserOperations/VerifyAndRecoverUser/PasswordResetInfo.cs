namespace Application.Processors.UserOperations.VerifyAndRecoverUser;
public class PasswordResetInfo
{
    public Guid UserId { get; set; }
    public string Hash { get; set; }
    public string Timestamp { get; set; }
    public bool Success { get; set; }
}


