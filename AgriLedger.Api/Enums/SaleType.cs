using System.Text.Json.Serialization;

namespace AgriLedger.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SaleType
{
    Wholesale = 1,
    Retail = 2
}
