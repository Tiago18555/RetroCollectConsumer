namespace Application.IgdbIntegrationOperations.SearchComputer;

public interface ISearchComputer
{
    Task<List<ComputerInfo>> RetrieveComputerInfoAsync(int game_id);

}
