using Domain;

namespace Application.IgdbIntegrationOperations.SearchGame;

public interface ISearchGame
{
    Task<ResponseModel> SearchBy(string name, string genre, string keyword, string companie, string language, string theme, string releaseyear, int limit);

    Task<ResponseModel> GetById(int id);

    Task<List<GameInfo>> RetrieveGameInfoAsync(int game_id);
}
