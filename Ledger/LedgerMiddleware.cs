using System;
using System.Threading.Tasks;
using DAL;
using Ledger.Requests;
using Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, IAppLogger appLogger)
        {
            var response = "";

            Console.WriteLine(
                $"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes). Client ip: {context.Connection.RemoteIpAddress}");

            await appLogger.DebugAsync("request",
                $"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes). Client ip: {context.Connection.RemoteIpAddress}");


            if (context.Request.Path.StartsWithSegments(_endpointPath + "/addr", StringComparison.Ordinal))
            {
                response = await RequestAddr.Response(dbContext, context);
            }

            if (context.Request.Path.StartsWithSegments(_endpointPath + "/ping", StringComparison.Ordinal))
            {
                response = await RequestPing.Response(dbContext, context);
            }


            // this has to be final for logging to work correctly
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/log", StringComparison.Ordinal))
            {
                response = await RequestLog.Response(dbContext);
            }
            else if (!string.IsNullOrWhiteSpace(response))
            {
                await appLogger.DebugAsync($"response - {context.Request.Path}", response);
            }


            if (!string.IsNullOrWhiteSpace(response))
            {
                await context.Response.WriteAsync(response);
            }

            // Call the next delegate/middleware in the pipeline
            if (_next != null)
            {
                await _next(context);
            }

            /*
            if (context.Request.Path.StartsWithSegments(_endpointPath, StringComparison.Ordinal))
            {
                await context.Response.WriteAsync("------- After Ledger ------");
            }
            */
        }
    }
}