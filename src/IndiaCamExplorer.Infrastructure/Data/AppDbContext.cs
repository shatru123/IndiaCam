using IndiaCamExplorer.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace IndiaCamExplorer.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Camera> Cameras => Set<Camera>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<SourceProvider> SourceProviders => Set<SourceProvider>();
    public DbSet<CameraHealthCheck> CameraHealthChecks => Set<CameraHealthCheck>();
    public DbSet<BrokenStreamReport> BrokenStreamReports => Set<BrokenStreamReport>();
    public DbSet<CameraSuggestion> CameraSuggestions => Set<CameraSuggestion>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.IconName).HasMaxLength(50);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.State).IsRequired().HasMaxLength(100);
            entity.Property(l => l.City).IsRequired().HasMaxLength(100);
            entity.Property(l => l.Region).IsRequired().HasMaxLength(50);
            entity.HasIndex(l => l.State);
            entity.HasIndex(l => new { l.State, l.City });
        });

        modelBuilder.Entity<Camera>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Slug).IsRequired().HasMaxLength(220);
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.Property(c => c.State).IsRequired().HasMaxLength(100);
            entity.Property(c => c.City).IsRequired().HasMaxLength(100);
            entity.Property(c => c.SourceUrl).IsRequired().HasMaxLength(500);
            entity.Property(c => c.StreamUrl).IsRequired().HasMaxLength(500);
            entity.Property(c => c.Attribution).IsRequired().HasMaxLength(200);
            entity.Property(c => c.ThumbnailUrl).HasMaxLength(500);

            entity.HasIndex(c => c.CategoryId);
            entity.HasIndex(c => c.State);
            entity.HasIndex(c => c.City);
            entity.HasIndex(c => c.IsActive);
            entity.HasIndex(c => c.HealthStatus);
            entity.HasIndex(c => new { c.IsActive, c.HealthStatus });

            entity.HasOne(c => c.Category)
                  .WithMany(cat => cat.Cameras)
                  .HasForeignKey(c => c.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.HealthChecks)
                  .WithOne(hc => hc.Camera)
                  .HasForeignKey(hc => hc.CameraId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Reports)
                  .WithOne(r => r.Camera)
                  .HasForeignKey(r => r.CameraId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SourceProvider>(entity =>
        {
            entity.HasKey(sp => sp.Id);
            entity.Property(sp => sp.ProviderId).IsRequired().HasMaxLength(100);
            entity.HasIndex(sp => sp.ProviderId).IsUnique();
            entity.Property(sp => sp.Name).IsRequired().HasMaxLength(150);
            entity.Property(sp => sp.WebsiteUrl).IsRequired().HasMaxLength(500);
            entity.Property(sp => sp.ProviderType).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<CameraHealthCheck>(entity =>
        {
            entity.HasKey(hc => hc.Id);
            entity.HasIndex(hc => hc.CameraId);
            entity.HasIndex(hc => hc.CheckedAt);
            entity.Property(hc => hc.ErrorMessage).HasMaxLength(500);
        });

        modelBuilder.Entity<BrokenStreamReport>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.CameraId);
            entity.HasIndex(r => r.Status);
            entity.Property(r => r.Reason).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Notes).HasMaxLength(1000);
            entity.Property(r => r.ReporterIpHash).HasMaxLength(64);
        });

        modelBuilder.Entity<CameraSuggestion>(entity =>
        {
            entity.HasKey(cs => cs.Id);
            entity.Property(cs => cs.Title).IsRequired().HasMaxLength(150);
            entity.Property(cs => cs.State).IsRequired().HasMaxLength(100);
            entity.Property(cs => cs.City).IsRequired().HasMaxLength(100);
            entity.Property(cs => cs.StreamUrl).IsRequired().HasMaxLength(500);
            entity.Property(cs => cs.SourceUrl).IsRequired().HasMaxLength(500);
            entity.Property(cs => cs.SubmitterName).HasMaxLength(100);
            entity.Property(cs => cs.SubmitterEmail).HasMaxLength(150);
            entity.HasIndex(cs => cs.Status);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.SessionId).IsRequired().HasMaxLength(100);
            entity.HasIndex(f => new { f.SessionId, f.CameraId }).IsUnique();
        });
    }
}
