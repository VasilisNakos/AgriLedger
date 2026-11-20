using System.Text.Json.Serialization;

namespace AgriLedger.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Paid = 1,
    Pending = 2
}
