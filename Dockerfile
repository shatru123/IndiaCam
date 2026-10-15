# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project definitions for efficient Docker layer caching
COPY IndiaCamExplorer.sln ./
COPY Directory.Build.props ./
COPY src/IndiaCamExplorer.Core/IndiaCamExplorer.Core.csproj src/IndiaCamExplorer.Core/
COPY src/IndiaCamExplorer.Infrastructure/IndiaCamExplorer.Infrastructure.csproj src/IndiaCamExplorer.Infrastructure/
COPY src/IndiaCamExplorer.Web/IndiaCamExplorer.Web.csproj src/IndiaCamExplorer.Web/
COPY tests/IndiaCamExplorer.Tests/IndiaCamExplorer.Tests.csproj tests/IndiaCamExplorer.Tests/

# Restore packages
RUN dotnet restore IndiaCamExplorer.sln

# Copy the rest of the source code
COPY . .

# Run test verification during build
RUN dotnet test tests/IndiaCamExplorer.Tests/IndiaCamExplorer.Tests.csproj -c Release --no-restore

# Publish the web application
RUN dotnet publish src/IndiaCamExplorer.Web/IndiaCamExplorer.Web.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

EXPOSE 10000
ENV PORT=10000
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published application artifacts
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IndiaCamExplorer.Web.dll"]
