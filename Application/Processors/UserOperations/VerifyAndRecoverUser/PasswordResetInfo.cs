namespace Application.Processors.UserOperations.VerifyAndRecoverUser;
public class PasswordResetInfo
{
    public Guid UserId { get; set; }
    public string Timestamphash { get; set; }
    public bool Success { get; set; }
}


