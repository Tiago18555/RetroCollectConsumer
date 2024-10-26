using Newtonsoft.Json;
using Infrastructure;
using Application.IgdbIntegrationOperations.Shared;

namespace Application.IgdbIntegrationOperations.SearchConsole;

public class SearchConsole : ISearchConsole
{
    private readonly HttpClient _httpClient;
    public SearchConsole()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<ConsoleInfo>> RetrieveConsoleInfoAsync(int game_id)
    {
        String query = @"
                fields                         
                name, 
                summary,
                platform_logo.image_id,
                category;
                where id = " + game_id.ToString() + ";";

        var res = await _httpClient.IgdbPostAsync<List<ConsoleInfo>>(query, "platforms");

        return res;
    }
}

public class ConsoleInfo
{
    [JsonProperty("id")]
    public int ConsoleId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("summary")]
    public string Description { get; set; }

    [JsonProperty("platform_logo")]
    private Platform_Logo Platform_Logo { get; set; }
    public string ImageUrl => Platform_Logo.Image_Id;

    [JsonProperty("category")]
    private int Category { get; set; }
    public bool IsPortable => Category == 5;
}
