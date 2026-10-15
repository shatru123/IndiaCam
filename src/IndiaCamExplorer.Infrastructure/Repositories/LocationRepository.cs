using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndiaCamExplorer.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;

    public LocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .OrderBy(l => l.State)
            .ThenBy(l => l.City)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAllStatesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .Select(l => l.State)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetCitiesByStateAsync(string state, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .Where(l => l.State == state)
            .Select(l => l.City)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }
}
