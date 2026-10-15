# IndiaCam Explorer

> **Explore India, Live.**  
> A high-performance, modern .NET 10 web application organizing verified, publicly accessible live streams, tourism webcams, and municipal traffic snapshot feeds across India.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4)](https://dotnet.microsoft.com/apps/aspnet)
[![EF Core](https://img.shields.io/badge/EF_Core-SQLite-blue)](https://docs.microsoft.com/ef/core/)
[![Leaflet](https://img.shields.io/badge/Leaflet-1.9.4-199900?logo=leaflet&logoColor=white)](https://leafletjs.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## 🌟 Overview

**IndiaCam Explorer** is a curated, transparent registry of real-time public broadcasts and viewcams across all **28 States and 8 Union Territories of India**. From sacred live Ganga Aartis along the Ghats of Varanasi to Himalayan panoramas in Shimla, coastal horizons in Goa, and smart-city transit nodes in Bengaluru, IndiaCam Explorer provides an intuitive, high-performance portal for exploring India in real time.

---

## ✨ Features

- **Exceptional UI & Theme Engine**:
  - Premium midnight-blue and charcoal surfaces (`#070b14`, `#0d1527`) with electric cyan accents (`#00f0ff`).
  - Seamless Light and Dark mode toggle with `localStorage` persistence and system preference sync.
  - Responsive 16:9 landscape camera cards with animated live status indicators, stream type badges, and hover lift effects.
- **Interactive India Map**:
  - Fullscreen Leaflet map centered on India with adaptive Dark/Light tile layers (CartoDB Dark Matter & Positron).
  - Custom electric cyan map pins displaying live status dots.
  - Interactive popup cards with thumbnail preview, title, location, and one-click stream playback.
  - Overlaid filter toolbar (filter markers live by category, state, and online status without page reload).
- **Multi-Protocol Video Player**:
  - **HLS (.m3u8)**: Powered by `Hls.js` with auto-recovery and native Safari fallback.
  - **Official Embeds**: Sandboxed YouTube live feeds and authorized broadcast players.
  - **Snapshot Viewcams**: Auto-refreshing image feeds (configurable: 15s, 30s, 60s) with live visual countdown progress bar and manual refresh.
  - Honest error diagnostics overlay when a feed is offline at source.
  - Theater mode and native fullscreen controls.
- **Search & Discovery Catalog**:
  - Full-text search matching camera title, city, state, and description.
  - Multi-dimensional filters for 7 categories, 36 States/UTs, stream types, and health statuses.
  - Server-side pagination and sorting (Popular, Recent, Name, State).
- **Favorites & Community Feeds**:
  - Client-side favorites dashboard stored in browser `localStorage` with server-side batch query sync.
  - Public camera submission form with automatic SSRF validation and admin review queue.
  - User-submitted broken stream reporting modal with instant persistence.
- **Protected Administrator Suite**:
  - Cookie-based authentication (`/Admin/Login`) protecting all administrative operations.
  - Metrics dashboard: Total feeds, online count, active providers, pending reports, and submissions.
  - Full Camera CRUD (create, update, delete, toggle active, trigger instant probe).
  - Source provider management with manual sync trigger.
  - Queue management for broken stream reports and user suggestions.
- **Automated Background Services**:
  - `CameraHealthCheckBackgroundService`: Automated periodic HTTP probes verifying stream availability, response time latency, and HTTP status codes.
  - `CameraSyncBackgroundService`: Scheduled provider discovery synchronizing registered public providers with URL deduplication.

---

## 🏛 Architecture

IndiaCam Explorer is architected cleanly following Clean Architecture principles:

```
IndiaCam/
├── IndiaCamExplorer.sln
├── Dockerfile
├── render.yaml
├── Directory.Build.props
├── src/
│   ├── IndiaCamExplorer.Core/           # Domain entities, Enums, DTOs, and Interfaces
│   │   ├── Entities/                    # Camera, Category, Location, SourceProvider, etc.
│   │   ├── Enums/                       # StreamType, HealthStatus, ReportStatus, etc.
│   │   ├── Interfaces/                  # ICameraRepository, ISecurityValidator, etc.
│   │   └── Models/                      # PagedResult, DTOs, Filter parameters
│   ├── IndiaCamExplorer.Infrastructure/ # EF Core, SQLite, Providers, Background Services
│   │   ├── Data/                        # AppDbContext, DbInitializer (Seed data)
│   │   ├── Repositories/                # CameraRepository, CategoryRepository, etc.
│   │   ├── Security/                    # UrlSecurityValidator (SSRF prevention)
│   │   ├── Services/                    # HealthCheckService (Safe HTTP probes)
│   │   ├── Providers/                   # Tourism, Scenic, Traffic, and Embed providers
│   │   └── BackgroundServices/          # Health check & provider discovery background services
│   └── IndiaCamExplorer.Web/            # ASP.NET Core MVC, Controllers, Views, UI Assets
│       ├── Controllers/                 # Home, Cameras, Categories, Map, Suggest, Admin
│       ├── Views/                       # Razor Views with midnight-blue styling
│       └── wwwroot/                     # Custom CSS, JS engines (site.js, player.js, map.js)
└── tests/
    └── IndiaCamExplorer.Tests/          # xUnit test suite (Security, Repositories, APIs)
```

---

## 🛡 Security & Ethical Standards

1. **Strictly Public & Authorized**: Only catalogs feeds openly published by state tourism departments, authorized temple trusts, public broadcasters (e.g. Doordarshan, ISRO), or municipal open-data portals.
2. **Zero Private Surveillance**: Never scans IP ranges, never indexes private CCTV, and never bypasses authentication.
3. **SSRF Defense (`UrlSecurityValidator`)**: Outbound probes reject loopback (`127.0.0.1`, `localhost`), private RFC 1918 ranges (`10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`), link-local / cloud metadata IPs (`169.254.169.254`), and non-HTTP/HTTPS schemes.
4. **Honest Status**: If a stream fails or is offline at source, an honest diagnostic state is displayed. Live viewer counts or statuses are never fabricated.

---

## 🚀 Local Development Setup (macOS / Linux / Windows)

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or .NET 9/8 compatible)
- Git

### Build & Run Instructions

```bash
# 1. Clone repository
git clone https://github.com/shatru123/IndiaCam.git
cd IndiaCam

# 2. Restore NuGet packages
dotnet restore

# 3. Build the solution
dotnet build

# 4. Run the xUnit test suite
dotnet test

# 5. Start the web application
dotnet run --project src/IndiaCamExplorer.Web
```

Open your browser at `http://localhost:5208` (or the URL printed in console).

### Default Administrator Credentials
- **Login URL**: `/Admin/Login`
- **Username**: `admin`
- **Password**: `IndiaCamAdmin2026!` *(Configurable via `appsettings.json` or environment variables)*

---

## ☁️ Deployment on Render

IndiaCam Explorer is pre-configured for zero-friction deployment on [Render](https://render.com).

### Option A: One-Click Blueprint (`render.yaml`)
1. Fork or push this repository to your GitHub account.
2. Log in to [Render Dashboard](https://dashboard.render.com).
3. Click **New +** > **Blueprint**.
4. Connect this repository; Render will automatically detect `render.yaml` and configure the service.
5. Click **Apply**.

### Option B: Docker Web Service
1. Create a new **Web Service** on Render pointing to your repository.
2. Select **Docker** as the runtime.
3. Render will build using the multi-stage [Dockerfile](Dockerfile) and expose port `10000`.

---

## 📄 License & Attribution

- **License**: Released under the [MIT License](LICENSE).
- **Copyright**: &copy; 2026 Shatrughna Ambhore.
- All live streams remain the copyright and property of their respective official broadcasters, tourism corporations, and temple trusts.
