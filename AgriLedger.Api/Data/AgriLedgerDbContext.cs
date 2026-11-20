using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Data;

public class AgriLedgerDbContext : DbContext
{
    public AgriLedgerDbContext(DbContextOptions<AgriLedgerDbContext> options)
        : base(options)
    {
    }

    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<Production> Productions => Set<Production>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgriLedgerDbContext).Assembly);
    }
}
