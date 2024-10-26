using Newtonsoft.Json;

namespace Application.IgdbIntegrationOperations.Shared;

public struct Cover
{
    [JsonIgnore]
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("image_id")]
    public string Image_Id { get; set; }
}
