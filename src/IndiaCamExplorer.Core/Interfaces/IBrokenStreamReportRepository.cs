using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Interfaces;

public interface IBrokenStreamReportRepository
{
    Task<BrokenStreamReport> AddAsync(BrokenStreamReport report, CancellationToken cancellationToken = default);
    Task<IEnumerable<BrokenStreamReport>> GetAllAsync(ReportStatus? status = null, CancellationToken cancellationToken = default);
    Task<BrokenStreamReport?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(BrokenStreamReport report, CancellationToken cancellationToken = default);
    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);
}
