using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndiaCamExplorer.Infrastructure.Repositories;

public class CameraSuggestionRepository : ICameraSuggestionRepository
{
    private readonly AppDbContext _context;

    public CameraSuggestionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CameraSuggestion> AddAsync(CameraSuggestion suggestion, CancellationToken cancellationToken = default)
    {
        suggestion.CreatedAt = DateTime.UtcNow;
        _context.CameraSuggestions.Add(suggestion);
        await _context.SaveChangesAsync(cancellationToken);
        return suggestion;
    }

    public async Task<IEnumerable<CameraSuggestion>> GetAllAsync(SuggestionStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.CameraSuggestions.AsQueryable();
        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<CameraSuggestion?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CameraSuggestions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task UpdateAsync(CameraSuggestion suggestion, CancellationToken cancellationToken = default)
    {
        _context.CameraSuggestions.Update(suggestion);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CameraSuggestions
            .CountAsync(s => s.Status == SuggestionStatus.Pending, cancellationToken);
    }
}
