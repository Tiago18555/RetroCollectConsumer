using Domain;

namespace Application.IgdbIntegrationOperations.SearchConsole;

public interface ISearchConsole
{
    Task<ResponseModel> SearchBy(string name, int limit);
    Task<ResponseModel> GetById(int id);
    Task<List<ConsoleInfo>> RetrieveConsoleInfoAsync(int game_id);
}
