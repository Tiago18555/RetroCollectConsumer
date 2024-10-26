using Newtonsoft.Json;

namespace Application.IgdbIntegrationOperations.Shared;

public struct Versions
{
    [JsonProperty("platform_version_release_dates")]
    private Platform_Version_Release_Dates Pvrd { get; set; }

    public readonly int Year => Pvrd.Y; 
}
