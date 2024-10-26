namespace Application.IgdbIntegrationOperations.SearchGame;

public interface ISearchGame
{
    Task<List<GameInfo>> RetrieveGameInfoAsync(int game_id);
}
