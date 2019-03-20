using Microsoft.AspNetCore.Builder;

namespace Ledger
{
    public static class LedgerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLedger(this IApplicationBuilder builder, string path, string publicKey, string privateKey)
        {
            return builder.UseMiddleware<LedgerMiddleware>(path, publicKey, privateKey);
        }
    }
}