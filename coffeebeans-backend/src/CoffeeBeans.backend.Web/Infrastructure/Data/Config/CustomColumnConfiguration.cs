using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeBeans.backend.Web.Domain.BeanAggregate;

namespace CoffeeBeans.backend.Web.Infrastructure.Data.Config;

public class CustomColumnConfiguration : IEntityTypeConfiguration<CustomColumn>
{
    public void Configure(EntityTypeBuilder<CustomColumn> builder)
    {
        builder.ToTable("CustomColumns");

        builder.Property(entity => entity.Id)
            .HasValueGenerator<VogenGuidIdValueGenerator<AppDbContext, CustomColumn, CustomColumnId>>()
            .HasVogenConversion()
            .IsRequired();

        builder.Property(x => x.ColumnName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DataType)
            .HasConversion<string>()
            .IsRequired();

        builder.Metadata
            .FindNavigation(nameof(CustomColumn.CustomValues))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.CustomValues)
            .WithOne(x => x.CustomColumn)
            .HasForeignKey(x => x.CustomColumnId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}