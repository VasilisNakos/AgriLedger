using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriLedger.Api.Data.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.Property(expense => expense.Amount).HasPrecision(18, 2);
        builder.Property(expense => expense.Supplier).HasMaxLength(200);
        builder.Property(expense => expense.Description).HasMaxLength(500);
        builder.Property(expense => expense.DocumentNumber).HasMaxLength(100);
        builder.HasIndex(expense => expense.Date);
        builder.HasIndex(expense => new { expense.CategoryId, expense.Date });
        builder.HasOne(expense => expense.Category)
            .WithMany(category => category.Expenses)
            .HasForeignKey(expense => expense.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
