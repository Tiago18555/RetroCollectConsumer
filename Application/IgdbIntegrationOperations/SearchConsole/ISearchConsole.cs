namespace Application.IgdbIntegrationOperations.SearchConsole;

public interface ISearchConsole
{
    Task<List<ConsoleInfo>> RetrieveConsoleInfoAsync(int game_id);
}
