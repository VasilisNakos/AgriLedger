using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AgriLedger.Api.Data;

public class AgriLedgerDbContextFactory : IDesignTimeDbContextFactory<AgriLedgerDbContext>
{
    public AgriLedgerDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AgriLedgerDbContext>()
            .UseSqlite("Data Source=agriledger.db")
            .Options;

        return new AgriLedgerDbContext(options);
    }
}
