using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeBeans.backend.Web.Domain.BeanAggregate;

namespace CoffeeBeans.backend.Web.Infrastructure.Data.Config;

public class BeanConfiguration : IEntityTypeConfiguration<Bean>
{
  public void Configure(EntityTypeBuilder<Bean> builder)
  {
    builder.ToTable("Beans");

    builder.Property(entity => entity.Id)
        .HasValueGenerator<VogenGuidIdValueGenerator<AppDbContext, Bean, BeanId>>()
        .HasVogenConversion()
        .IsRequired();

    builder.Property(x => x.TastingProfile)
        .IsRequired()
        .HasMaxLength(500);

    builder.Property(x => x.BagWeightG)
        .IsRequired();

    builder.Property(x => x.RoastIndex)
        .IsRequired();

    builder.Property(x => x.NumFarms)
        .IsRequired();

    builder.Property(x => x.NumAcidityNotes)
        .IsRequired();

    builder.Property(x => x.NumSweetnessNotes)
        .IsRequired();

    builder.Property(x => x.X)
        .IsRequired();

    builder.Property(x => x.Y)
        .IsRequired();

    builder.Metadata
        .FindNavigation(nameof(Bean.CustomValues))!
        .SetPropertyAccessMode(PropertyAccessMode.Field);

    builder.HasMany(x => x.CustomValues)
        .WithOne(x => x.Bean)
        .HasForeignKey(x => x.BeanId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}