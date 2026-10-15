using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndiaCamExplorer.Infrastructure.Repositories;

public class SourceProviderRepository : ISourceProviderRepository
{
    private readonly AppDbContext _context;

    public SourceProviderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SourceProvider>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SourceProviders
            .OrderBy(sp => sp.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<SourceProvider?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SourceProviders.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<SourceProvider?> GetByProviderIdAsync(string providerId, CancellationToken cancellationToken = default)
    {
        return await _context.SourceProviders
            .FirstOrDefaultAsync(sp => sp.ProviderId == providerId, cancellationToken);
    }

    public async Task UpdateAsync(SourceProvider provider, CancellationToken cancellationToken = default)
    {
        _context.SourceProviders.Update(provider);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
