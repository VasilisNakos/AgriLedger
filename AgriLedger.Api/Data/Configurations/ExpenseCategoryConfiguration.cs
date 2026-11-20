using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriLedger.Api.Data.Configurations;

public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public static readonly DateTime SeedCreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("ExpenseCategories");
        builder.Property(category => category.Code).HasMaxLength(32).IsRequired();
        builder.Property(category => category.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(category => category.Code).IsUnique();

        builder.HasData(
            new ExpenseCategory
            {
                Id = 1,
                Code = "FEED",
                Name = "Ζωοτροφές",
                IsActive = true,
                SortOrder = 1,
                CreatedAt = SeedCreatedAt
            },
            new ExpenseCategory
            {
                Id = 2,
                Code = "ELECTRICITY",
                Name = "Ρεύμα / Ενέργεια",
                IsActive = true,
                SortOrder = 2,
                CreatedAt = SeedCreatedAt
            },
            new ExpenseCategory
            {
                Id = 3,
                Code = "PAYROLL",
                Name = "Μισθοδοσία",
                IsActive = true,
                SortOrder = 3,
                CreatedAt = SeedCreatedAt
            },
            new ExpenseCategory
            {
                Id = 4,
                Code = "PURCHASES",
                Name = "Αγορές πουλάδων / εξοπλισμού",
                IsActive = true,
                SortOrder = 4,
                CreatedAt = SeedCreatedAt
            },
            new ExpenseCategory
            {
                Id = 5,
                Code = "LOGISTICS",
                Name = "Καύσιμα / Μεταφορικά",
                IsActive = true,
                SortOrder = 5,
                CreatedAt = SeedCreatedAt
            },
            new ExpenseCategory
            {
                Id = 6,
                Code = "OTHER",
                Name = "Λοιπά",
                IsActive = true,
                SortOrder = 6,
                CreatedAt = SeedCreatedAt
            });
    }
}
