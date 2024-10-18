using Domain;

namespace Application.IgdbIntegrationOperations.SearchComputer;

public interface ISearchComputer
{
    Task<ResponseModel> SearchBy(string name, int limit);
    Task<ResponseModel> GetById(int id);
    Task<List<ComputerInfo>> RetrieveComputerInfoAsync(int game_id);

}
