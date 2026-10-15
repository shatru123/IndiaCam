using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndiaCamExplorer.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<CategoryCountDto>> GetCategoriesWithCountsAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);

        var counts = await _context.Cameras
            .Where(c => c.IsActive)
            .GroupBy(c => c.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                Total = g.Count(),
                Online = g.Count(c => c.HealthStatus == HealthStatus.Online)
            })
            .ToListAsync(cancellationToken);

        var countMap = counts.ToDictionary(c => c.CategoryId);

        return categories.Select(cat =>
        {
            countMap.TryGetValue(cat.Id, out var stat);
            return new CategoryCountDto
            {
                Id = cat.Id,
                Name = cat.Name,
                Slug = cat.Slug,
                Description = cat.Description,
                IconName = cat.IconName,
                CameraCount = stat?.Total ?? 0,
                OnlineCount = stat?.Online ?? 0
            };
        });
    }
}
