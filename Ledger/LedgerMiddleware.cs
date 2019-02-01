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
            
            Console.WriteLine($"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes)");

            await context.Response.WriteAsync("------- Before ------ \n\r");

            // Call the next delegate/middleware in the pipeline
            await _next(context);
            
            await context.Response.WriteAsync("\n\r------- After ------");
            
        }
    }
}