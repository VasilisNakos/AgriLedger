namespace AgriLedger.Api.Services;

internal static class EntryRules
{
    public static void EnsureDate(DateOnly date)
    {
        if (date.Year is < 2000 or > 2100)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Η ημερομηνία είναι εκτός αποδεκτού εύρους.");
        }
    }

    public static void EnsureOptionalLength(string? value, int maxLength, string message)
    {
        if (value is not null && value.Trim().Length > maxLength)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, message);
        }
    }

    public static string? Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    public static void EnsureDefined<TEnum>(TEnum value, string message)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, message);
        }
    }
}
