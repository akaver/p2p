using Microsoft.AspNetCore.Builder;

namespace Ledger
{
    public static class LedgerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLedger(this IApplicationBuilder builder, string path)
        {
            return builder.UseMiddleware<LedgerMiddleware>(path);
        }
    }
}