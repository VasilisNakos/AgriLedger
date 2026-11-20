using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Data;

public static class DbProviderSetup
{
    public const string SqliteProvider = "Sqlite";

    public static IServiceCollection AddAgriLedgerDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? SqliteProvider;
        var connectionString = configuration.GetConnectionString("AgriLedger")
            ?? "Data Source=agriledger.db";

        services.AddDbContext<AgriLedgerDbContext>(options =>
        {
            if (provider.Equals(SqliteProvider, StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlite(connectionString);
                return;
            }

            throw new InvalidOperationException(
                $"Database provider '{provider}' is not set up. The supported provider is '{SqliteProvider}'.");
        });

        return services;
    }
}
