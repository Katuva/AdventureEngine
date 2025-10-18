using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AdventureEngine.Data;

/// <summary>
/// Factory for creating DbContext instances at design time (for migrations)
/// Instantiated by Entity Framework Core tooling via reflection
/// </summary>
// ReSharper disable once UnusedType.Global
public class AdventureDbContextFactory : IDesignTimeDbContextFactory<AdventureDbContext>
{
    public AdventureDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AdventureDbContext>();
        optionsBuilder.UseSqlite("Data Source=adventure.db");

        return new AdventureDbContext(optionsBuilder.Options);
    }
}
