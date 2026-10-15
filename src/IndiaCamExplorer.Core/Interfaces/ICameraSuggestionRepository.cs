using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Interfaces;

public interface ICameraSuggestionRepository
{
    Task<CameraSuggestion> AddAsync(CameraSuggestion suggestion, CancellationToken cancellationToken = default);
    Task<IEnumerable<CameraSuggestion>> GetAllAsync(SuggestionStatus? status = null, CancellationToken cancellationToken = default);
    Task<CameraSuggestion?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(CameraSuggestion suggestion, CancellationToken cancellationToken = default);
    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);
}
