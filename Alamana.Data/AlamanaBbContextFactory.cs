using System.Text.Json;
using Alamana.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Alamana.Data;

/// <summary>
/// EF Core tools (migrations) use this instead of the web app at runtime.
/// It must use the <b>same</b> SQL connection as your API, otherwise <c>database update</c>
/// applies migrations to the wrong database (e.g. LocalDB) while the site uses hosting SQL.
/// </summary>
public class AlamanaBbContextFactory : IDesignTimeDbContextFactory<AlamanaBbContext>
{
    public AlamanaBbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<AlamanaBbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AlamanaBbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        var fromEnv = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("EFCORETOOLS_CONNECTION");

        if (!string.IsNullOrWhiteSpace(fromEnv))
            return fromEnv;

        var webDir = FindWebProjectDirectoryContainingAppsettings()
            ?? throw new InvalidOperationException(
                "Could not find Alamana/appsettings.json. Run dotnet ef from the backend folder (parent of Alamana and Alamana.Data), " +
                "or set environment variable ConnectionStrings__DefaultConnection to your SQL connection string.");

        return ReadDefaultConnection(webDir);
    }

    private static string? FindWebProjectDirectoryContainingAppsettings()
    {
        for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "Alamana", "appsettings.json");
            if (File.Exists(candidate))
                return Path.Combine(dir.FullName, "Alamana");
        }

        return null;
    }

    private static string ReadDefaultConnection(string webProjectDir)
    {
        var mainPath = Path.Combine(webProjectDir, "appsettings.json");
        if (!File.Exists(mainPath))
            throw new FileNotFoundException($"Missing {mainPath}.");

        var connectionString = ReadDefaultConnectionFromJsonFile(mainPath);

        var devPath = Path.Combine(webProjectDir, "appsettings.Development.json");
        if (File.Exists(devPath))
        {
            var devCs = ReadDefaultConnectionFromJsonFile(devPath);
            if (!string.IsNullOrWhiteSpace(devCs))
                connectionString = devCs;
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"ConnectionStrings:DefaultConnection is missing in {mainPath}. Add it or set ConnectionStrings__DefaultConnection.");
        }

        return connectionString;
    }

    private static string? ReadDefaultConnectionFromJsonFile(string path)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        if (!doc.RootElement.TryGetProperty("ConnectionStrings", out var csObj))
            return null;

        return csObj.TryGetProperty("DefaultConnection", out var el) ? el.GetString() : null;
    }
}
