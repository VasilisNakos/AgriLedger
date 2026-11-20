using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace AgriLedger.Api;

public sealed class LedgerExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not LedgerException ledger)
        {
            return false;
        }

        await Results.Problem(detail: ledger.Message, statusCode: ledger.StatusCode)
            .ExecuteAsync(httpContext);
        return true;
    }
}
