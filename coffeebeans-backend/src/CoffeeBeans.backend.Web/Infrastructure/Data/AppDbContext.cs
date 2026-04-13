using System.Reflection;
using Microsoft.EntityFrameworkCore;
using CoffeeBeans.backend.Web.Domain.BeanAggregate;

namespace CoffeeBeans.backend.Web.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : 
  DbContext(options)
{
  public DbSet<Bean> Beans => Set<Bean>();
  public DbSet<CustomColumn> CustomColumns => Set<CustomColumn>();
  public DbSet<BeanCustomValue> BeanCustomValues => Set<BeanCustomValue>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
