using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Core.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryCountDto>> GetCategoriesWithCountsAsync(CancellationToken cancellationToken = default);
}
