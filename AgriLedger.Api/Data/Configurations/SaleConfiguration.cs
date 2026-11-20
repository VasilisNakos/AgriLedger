using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriLedger.Api.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.Property(sale => sale.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(sale => sale.SaleType).HasConversion<string>().HasMaxLength(16);
        builder.Property(sale => sale.EggSize).HasConversion<string>().HasMaxLength(16);
        builder.Property(sale => sale.PaymentStatus).HasConversion<string>().HasMaxLength(16);
        builder.Property(sale => sale.PricePerCase).HasPrecision(18, 2);
        builder.Property(sale => sale.TotalAmount).HasPrecision(18, 2);
        builder.HasIndex(sale => sale.Date);
        builder.HasIndex(sale => new { sale.PaymentStatus, sale.Date });
    }
}
