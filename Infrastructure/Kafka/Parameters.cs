namespace Infrastructure.Kafka;

public class Parameters
{
    public Parameters()
    {
        BootstrapServer = "kafka:29092";
        GroupId = "Group 1";
    }

    public string BootstrapServer { get; set; }
    public string GroupId { get; set; }
}
