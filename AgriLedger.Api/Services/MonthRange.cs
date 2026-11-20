namespace AgriLedger.Api.Services;

public static class MonthRange
{
    public static (DateOnly? Start, DateOnly? End) Resolve(int? year, int? month)
    {
        if (year is null && month is null)
        {
            return (null, null);
        }

        if (year is null || month is null || month is < 1 or > 12 || year is < 2000 or > 2100)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Δώσε έγκυρο έτος και μήνα μαζί.");
        }

        var start = new DateOnly(year.Value, month.Value, 1);
        return (start, start.AddMonths(1));
    }
}
