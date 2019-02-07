using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tor
{
    public class TorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _endpointPath;

        public TorMiddleware(RequestDelegate next, string path)
        {
            _next = next;
            _endpointPath = path;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine(
                $"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes)");

            if (context.Request.Path.Equals(_endpointPath, StringComparison.Ordinal))
            {
                await context.Response.WriteAsync("------- Before Tor ------ \n\r");
            }

            // Call the next delegate/middleware in the pipeline
            if (_next != null)
            {
                await _next(context);
            }


            if (context.Request.Path.Equals(_endpointPath, StringComparison.Ordinal))
            {
                await context.Response.WriteAsync("\n\r------- After Tor ------");
            }
        }
    }
}