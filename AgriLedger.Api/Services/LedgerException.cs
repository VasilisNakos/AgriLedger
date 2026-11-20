namespace AgriLedger.Api.Services;

public sealed class LedgerException : Exception
{
    public LedgerException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
