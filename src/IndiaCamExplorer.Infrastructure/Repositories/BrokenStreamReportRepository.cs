using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndiaCamExplorer.Infrastructure.Repositories;

public class BrokenStreamReportRepository : IBrokenStreamReportRepository
{
    private readonly AppDbContext _context;

    public BrokenStreamReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BrokenStreamReport> AddAsync(BrokenStreamReport report, CancellationToken cancellationToken = default)
    {
        report.ReportedAt = DateTime.UtcNow;
        _context.BrokenStreamReports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<IEnumerable<BrokenStreamReport>> GetAllAsync(ReportStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.BrokenStreamReports
            .Include(r => r.Camera)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query
            .OrderByDescending(r => r.ReportedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<BrokenStreamReport?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.BrokenStreamReports
            .Include(r => r.Camera)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(BrokenStreamReport report, CancellationToken cancellationToken = default)
    {
        _context.BrokenStreamReports.Update(report);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BrokenStreamReports
            .CountAsync(r => r.Status == ReportStatus.Pending, cancellationToken);
    }
}
