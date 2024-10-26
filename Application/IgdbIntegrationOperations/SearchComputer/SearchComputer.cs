using Newtonsoft.Json;
using Infrastructure;
using Application.IgdbIntegrationOperations.Shared;

namespace Application.IgdbIntegrationOperations.SearchComputer;

public class SearchComputer : ISearchComputer
{
    private readonly HttpClient _httpClient;
    public SearchComputer()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<ComputerInfo>> RetrieveComputerInfoAsync(int game_id)
    {
        String query = @"
                fields                         
                name, 
                summary,
                platform_logo.image_id,
                category;
                where id = " + game_id.ToString() + ";";

        var res = await _httpClient.IgdbPostAsync<List<ComputerInfo>>(query, "platforms");

        return res;
    }
}
public class ComputerInfo
{
    [JsonProperty("id")]
    public int ComputerId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("summary")]
    public string Description { get; set; }

    [JsonProperty("platform_logo")]
    private Platform_Logo Platform_Logo { get; set; }
    public string ImageUrl => Platform_Logo.Image_Id;

    [JsonProperty("category")]
    private int Category { get; set; }
    public bool IsArcade => Category == 2;
}
