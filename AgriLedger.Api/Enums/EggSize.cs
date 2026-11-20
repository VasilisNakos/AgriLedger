using System.Text.Json.Serialization;

namespace AgriLedger.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EggSize
{
    Small = 1,
    Medium = 2,
    Large = 3,
    XL = 4
}
