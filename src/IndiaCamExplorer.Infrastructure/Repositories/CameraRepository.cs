using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndiaCamExplorer.Infrastructure.Repositories;

public class CameraRepository : ICameraRepository
{
    private readonly AppDbContext _context;

    public CameraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Camera>> GetCamerasAsync(CameraFilterParams filterParams, CancellationToken cancellationToken = default)
    {
        var query = _context.Cameras
            .Include(c => c.Category)
            .Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(filterParams.Search))
        {
            var search = filterParams.Search.Trim().ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(search) ||
                                     c.City.ToLower().Contains(search) ||
                                     c.State.ToLower().Contains(search) ||
                                     (c.Description != null && c.Description.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(filterParams.CategorySlug))
        {
            query = query.Where(c => c.Category.Slug == filterParams.CategorySlug);
        }

        if (!string.IsNullOrWhiteSpace(filterParams.State))
        {
            query = query.Where(c => c.State == filterParams.State);
        }

        if (!string.IsNullOrWhiteSpace(filterParams.City))
        {
            query = query.Where(c => c.City == filterParams.City);
        }

        if (filterParams.StreamType.HasValue)
        {
            query = query.Where(c => c.StreamType == filterParams.StreamType.Value);
        }

        if (filterParams.HealthStatus.HasValue)
        {
            query = query.Where(c => c.HealthStatus == filterParams.HealthStatus.Value);
        }

        query = filterParams.SortBy?.ToLower() switch
        {
            "recent" => query.OrderByDescending(c => c.CreatedAt),
            "name" => query.OrderBy(c => c.Title),
            "state" => query.OrderBy(c => c.State).ThenBy(c => c.City),
            _ => query.OrderByDescending(c => c.HealthStatus == HealthStatus.Online)
                      .ThenByDescending(c => c.IsVerified)
                      .ThenByDescending(c => c.CreatedAt)
        };

        var totalItems = await query.CountAsync(cancellationToken);
        var page = Math.Max(1, filterParams.Page);
        var pageSize = filterParams.PageSize > 0 ? filterParams.PageSize : 12;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Camera>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Camera?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Cameras
            .Include(c => c.Category)
            .Include(c => c.HealthChecks.OrderByDescending(h => h.CheckedAt).Take(10))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Camera?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Cameras
            .Include(c => c.Category)
            .Include(c => c.HealthChecks.OrderByDescending(h => h.CheckedAt).Take(10))
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Camera>> GetFeaturedCamerasAsync(int count = 6, CancellationToken cancellationToken = default)
    {
        return await _context.Cameras
            .Include(c => c.Category)
            .Where(c => c.IsActive && c.IsVerified && c.HealthStatus == HealthStatus.Online)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Camera>> GetRecentlyAddedCamerasAsync(int count = 6, CancellationToken cancellationToken = default)
    {
        return await _context.Cameras
            .Include(c => c.Category)
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CameraMarkerDto>> GetMarkersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cameras
            .Include(c => c.Category)
            .Where(c => c.IsActive)
            .Select(c => new CameraMarkerDto
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                CategoryName = c.Category.Name,
                State = c.State,
                City = c.City,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                StreamType = c.StreamType,
                HealthStatus = c.HealthStatus,
                ThumbnailUrl = c.ThumbnailUrl
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StateSummaryDto>> GetStateSummariesAsync(CancellationToken cancellationToken = default)
    {
        var locations = await _context.Locations.ToListAsync(cancellationToken);
        var cameraGroupings = await _context.Cameras
            .Where(c => c.IsActive)
            .GroupBy(c => c.State)
            .Select(g => new
            {
                State = g.Key,
                Total = g.Count(),
                Online = g.Count(c => c.HealthStatus == HealthStatus.Online)
            })
            .ToListAsync(cancellationToken);

        var map = cameraGroupings.ToDictionary(g => g.State, StringComparer.OrdinalIgnoreCase);

        return locations
            .GroupBy(l => l.State)
            .Select(g =>
            {
                var first = g.First();
                map.TryGetValue(first.State, out var counts);
                return new StateSummaryDto
                {
                    State = first.State,
                    Region = first.Region,
                    Latitude = first.Latitude,
                    Longitude = first.Longitude,
                    TotalCameras = counts?.Total ?? 0,
                    OnlineCameras = counts?.Online ?? 0
                };
            })
            .OrderByDescending(s => s.TotalCameras)
            .ThenBy(s => s.State)
            .ToList();
    }

    public async Task<IEnumerable<CameraHealthCheck>> GetRecentHealthChecksAsync(int cameraId, int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.CameraHealthChecks
            .Where(h => h.CameraId == cameraId)
            .OrderByDescending(h => h.CheckedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cameras.CountAsync(c => c.IsActive, cancellationToken);
    }

    public async Task<int> GetOnlineCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cameras.CountAsync(c => c.IsActive && c.HealthStatus == HealthStatus.Online, cancellationToken);
    }

    public async Task<Camera> AddAsync(Camera camera, CancellationToken cancellationToken = default)
    {
        camera.CreatedAt = DateTime.UtcNow;
        camera.UpdatedAt = DateTime.UtcNow;
        _context.Cameras.Add(camera);
        await _context.SaveChangesAsync(cancellationToken);
        return camera;
    }

    public async Task UpdateAsync(Camera camera, CancellationToken cancellationToken = default)
    {
        camera.UpdatedAt = DateTime.UtcNow;
        _context.Cameras.Update(camera);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var camera = await _context.Cameras.FindAsync(new object[] { id }, cancellationToken);
        if (camera != null)
        {
            _context.Cameras.Remove(camera);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
