
using Application.IgdbIntegrationOperations.Shared;
using Newtonsoft.Json;

namespace Application.IgdbIntegrationOperations.SearchGame;
public class GameInfo
{
    [JsonProperty("id")]
    public int GameId { get; set; }

    [JsonProperty("first_release_date")]
    public int FirstReleaseDate { get; set; }

    [JsonProperty("genres")]
    private List<Genre> _Genres { get; set; }
    public List<string> Genres => _Genres?.Select(prop => prop.Name).ToList();

    [JsonProperty("name")]
    public string Title { get; set; }

    [JsonProperty("platforms")]
    public List<Platform> Platforms { get; set; }

    [JsonProperty("storyline")]
    public string Description { get; set; }

    [JsonProperty("summary")]
    public string Summary { get; set; }

    [JsonProperty("cover")]
    private Cover _Cover { get; set; }
    public string Cover => _Cover.Image_Id ?? "";
}
public struct Genre
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public struct Platform
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("platform_logo")]
    private PlatformLogo _PlatformLogo { get; set; }
    public string PlatformLogo => _PlatformLogo.ImageId ?? "";
}

public struct PlatformLogo
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("image_id")]
    public string ImageId { get; set; }
}
