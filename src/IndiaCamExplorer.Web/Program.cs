using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.BackgroundServices;
using IndiaCamExplorer.Infrastructure.Data;
using IndiaCamExplorer.Infrastructure.Providers;
using IndiaCamExplorer.Infrastructure.Repositories;
using IndiaCamExplorer.Infrastructure.Security;
using IndiaCamExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Bind to Render / Cloud dynamic PORT if provided
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add Database Context with directory auto-creation for persistent disks
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=IndiaCam.db";
if (connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
{
    var dbPath = connectionString.Substring("Data Source=".Length).Trim();
    var dir = Path.GetDirectoryName(dbPath);
    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
    {
        Directory.CreateDirectory(dir);
    }
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.Cookie.Name = "IndiaCam.Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Add HTTP Client Factory
builder.Services.AddHttpClient("CameraHealthProber", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

// Register Domain & Infrastructure Services
builder.Services.AddSingleton<ISecurityValidator, UrlSecurityValidator>();
builder.Services.AddScoped<ICameraRepository, CameraRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ISourceProviderRepository, SourceProviderRepository>();
builder.Services.AddScoped<IBrokenStreamReportRepository, BrokenStreamReportRepository>();
builder.Services.AddScoped<ICameraSuggestionRepository, CameraSuggestionRepository>();
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

// Register Camera Source Providers
builder.Services.AddSingleton<ICameraSourceProvider, TourismLivestreamProvider>();
builder.Services.AddSingleton<ICameraSourceProvider, PublicScenicWebcamProvider>();
builder.Services.AddSingleton<ICameraSourceProvider, PublicTrafficCameraProvider>();
builder.Services.AddSingleton<ICameraSourceProvider, OfficialEmbedLivestreamProvider>();

// Register Background Services
builder.Services.AddSingleton<CameraSyncBackgroundService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<CameraSyncBackgroundService>());
builder.Services.AddHostedService<CameraHealthCheckBackgroundService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbInitializer.InitializeAsync(dbContext, logger);
}

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Custom Friendly Routes
app.MapControllerRoute(
    name: "camera_details",
    pattern: "cameras/{slug}",
    defaults: new { controller = "Cameras", action = "Details" });

app.MapControllerRoute(
    name: "camera_state",
    pattern: "cameras/state/{state}",
    defaults: new { controller = "Cameras", action = "State" });

app.MapControllerRoute(
    name: "camera_category",
    pattern: "cameras/category/{slug}",
    defaults: new { controller = "Cameras", action = "Category" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
