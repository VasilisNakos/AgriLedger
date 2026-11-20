using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriLedger.Api.Data.Configurations;

public class ProductionConfiguration : IEntityTypeConfiguration<Production>
{
    public void Configure(EntityTypeBuilder<Production> builder)
    {
        builder.ToTable("Productions");
        builder.Property(production => production.Notes).HasMaxLength(500);
        builder.HasIndex(production => production.Date);
    }
}
