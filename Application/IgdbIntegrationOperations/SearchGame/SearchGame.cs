using Infrastructure;

namespace Application.IgdbIntegrationOperations.SearchGame;

public class SearchGame : ISearchGame
{
    private readonly HttpClient _httpClient;

    public SearchGame()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<GameInfo>> RetrieveGameInfoAsync(int game_id)
    {
        String query = @"
                fields
                first_release_date, 
                genres.name,  
                name, 
                platforms.name, platforms.platform_logo.image_id,
                storyline, 
                summary,
                cover.image_id; 
                where id = " + game_id.ToString() + ";";

        var res = await _httpClient.IgdbPostAsync<List<GameInfo>>(query, "games");

        return res;
    }
}
