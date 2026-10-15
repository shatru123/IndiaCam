using IndiaCamExplorer.Core.Entities;

namespace IndiaCamExplorer.Core.Interfaces;

public interface ILocationRepository
{
    Task<IEnumerable<Location>> GetAllLocationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAllStatesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetCitiesByStateAsync(string state, CancellationToken cancellationToken = default);
}
