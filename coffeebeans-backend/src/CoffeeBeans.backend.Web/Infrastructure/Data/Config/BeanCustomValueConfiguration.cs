using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeBeans.backend.Web.Domain.BeanAggregate;

namespace CoffeeBeans.backend.Web.Infrastructure.Data.Config;

public class BeanCustomValueConfiguration : IEntityTypeConfiguration<BeanCustomValue>
{
  public void Configure(EntityTypeBuilder<BeanCustomValue> builder)
  {
    builder.ToTable("BeanCustomValues");

    builder.HasKey(x => new { x.BeanId, x.CustomColumnId });

    builder.Property(x => x.BeanId)
        .HasVogenConversion()
        .IsRequired();

    builder.Property(x => x.CustomColumnId)
        .HasVogenConversion()
        .IsRequired();

    builder.Property(x => x.Value)
        .HasMaxLength(500)
        .IsRequired();

    builder.HasOne(x => x.Bean)
        .WithMany(x => x.CustomValues)
        .HasForeignKey(x => x.BeanId)
        .HasPrincipalKey(x => x.Id)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(x => x.CustomColumn)
        .WithMany(x => x.CustomValues)
        .HasForeignKey(x => x.CustomColumnId)
        .HasPrincipalKey(x => x.Id)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
