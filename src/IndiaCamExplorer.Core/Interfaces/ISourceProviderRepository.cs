using IndiaCamExplorer.Core.Entities;

namespace IndiaCamExplorer.Core.Interfaces;

public interface ISourceProviderRepository
{
    Task<IEnumerable<SourceProvider>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SourceProvider?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SourceProvider?> GetByProviderIdAsync(string providerId, CancellationToken cancellationToken = default);
    Task UpdateAsync(SourceProvider provider, CancellationToken cancellationToken = default);
}
