using System.Globalization;
using AgriLedger.Api.Enums;

namespace AgriLedger.Api.Pages;

public static class UiText
{
    private static readonly CultureInfo Greek = CultureInfo.GetCultureInfo("el-GR");

    public static string Money(decimal amount) => amount.ToString("C", Greek);

    public static string Cost(decimal amount) => amount.ToString("N4", Greek) + " €";

    public static string Percent(decimal amount) => amount.ToString("N2", Greek) + " %";

    public static string MonthName(int month)
    {
        var name = Greek.DateTimeFormat.GetMonthName(month);
        return char.ToUpper(name[0], Greek) + name[1..];
    }

    public static string SaleTypeName(SaleType type) => type switch
    {
        SaleType.Wholesale => "Χονδρική",
        SaleType.Retail => "Λιανική",
        _ => type.ToString()
    };

    public static string EggSizeName(EggSize size) => size switch
    {
        EggSize.Small => "Μικρά",
        EggSize.Medium => "Μεσαία",
        EggSize.Large => "Μεγάλα",
        EggSize.XL => "Πολύ μεγάλα",
        _ => size.ToString()
    };

    public static string PaymentName(PaymentStatus status) => status switch
    {
        PaymentStatus.Paid => "Πληρωμένη",
        PaymentStatus.Pending => "Εκκρεμεί",
        _ => status.ToString()
    };
}
