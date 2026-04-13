using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoffeeBeans.backend.Web.Infrastructure.Data;

/// <summary>
/// Design-time factory for creating AppDbContext instances for EF Core tools.
/// Used by commands such as:
/// - dotnet ef migrations add
/// - dotnet ef database update
///
/// This factory is only for EF Core design-time operations.
/// Runtime database configuration is handled by the application's normal service registration.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__AppDb")
            ?? "server=mssql;port=3306;database=CoffeeBeansDb;user=root;password=YourStrong!Passw0rd;";

        optionsBuilder.UseMySQL(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}