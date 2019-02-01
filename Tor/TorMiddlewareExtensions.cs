using Microsoft.AspNetCore.Builder;

namespace Tor
{
    public static class TorMiddlewareExtensions
    {
        public static IApplicationBuilder UseTor(this IApplicationBuilder builder, string path)
        {
            return builder.UseMiddleware<TorMiddleware>(path);
        }
    }
}