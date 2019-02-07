using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ledger
{
    public class LedgerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _endpointPath;

        public LedgerMiddleware(RequestDelegate next, string path)
        {
            _next = next;
            _endpointPath = path;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine(
                $"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes). Client ip: {context.Connection.RemoteIpAddress}");

            if (context.Request.Path.Equals(_endpointPath, StringComparison.Ordinal))
            {
                await context.Response.WriteAsync("------- Before Ledger ------\n");
            }


            // Call the next delegate/middleware in the pipeline
            if (_next != null)
            {
                await _next(context);
            }

            if (context.Request.Path.Equals(_endpointPath, StringComparison.Ordinal))
            {
                await context.Response.WriteAsync("------- After Ledger ------");
            }
        }
    }
}