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
        private readonly string _publicKey;
        private readonly string _privateKey;
        
        private readonly object dbLock = new object();
        

        public LedgerMiddleware(RequestDelegate next, string path, string publicKey, string privateKey)
        {
            _next = next;
            _endpointPath = path;
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, IAppLogger appLogger)
        {
            var response = "";

            
            // log only this endpoint traffic
            if (context.Request.Path.StartsWithSegments(_endpointPath, StringComparison.Ordinal))
            {
                Console.WriteLine(
                    $"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes). Client ip: {context.Connection.RemoteIpAddress}");

                await appLogger.DebugAsync("request",
                    $"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes). Client ip: {context.Connection.RemoteIpAddress}");
            }

            
            // p2p responses - who do we know already
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/addr", StringComparison.Ordinal))
            {
                response = await RequestAddr.Response(dbContext, context);
            }

            // p2p responses - ping
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/ping", StringComparison.Ordinal))
            {
                response = await RequestPing.Response(dbContext, context, _publicKey);
            }

            
            // ledger - get blocks from start to end 
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/blocks", StringComparison.Ordinal))
            {
                response = await RequestBlocks.Response(dbContext, context);
            }

            // ledger - merkle root of current ledger
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/info", StringComparison.Ordinal))
            {
                response = await RequestInfo.Response(dbContext, context);
            }

            
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/singleblock", StringComparison.Ordinal))
            {
                response = await RequestSingleBlock.Response(dbContext, context);
            }

            if (context.Request.Path.StartsWithSegments(_endpointPath + "/createblock", StringComparison.Ordinal) ) //  && context.Request.Method == "POST"
            {
                response = await RequestCreateBlock.Response(dbContext, context, _publicKey, _privateKey, dbLock);
            }

            if (context.Request.Path.StartsWithSegments(_endpointPath + "/receiveblock", StringComparison.Ordinal) && context.Request.Method == "POST")  
            {
                response = await RequestReceiveBlock.Response(dbContext, context, _publicKey, _privateKey, dbLock);
            }

            
            
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/receiveledger", StringComparison.Ordinal) && context.Request.Method == "POST")  
            {
                response = await RequestReceiveLedger.Response(dbContext, context, _publicKey, _privateKey, dbLock);
            }

            // ledger - get one specific block
            
            
            // this has to be final for logging to work correctly
            if (context.Request.Path.StartsWithSegments(_endpointPath + "/log", StringComparison.Ordinal))
            {
                response = await RequestLog.Response(dbContext);
            }
            else if (!string.IsNullOrWhiteSpace(response))
            {
                await appLogger.DebugAsync($"response - {context.Request.Path}", response);
            }
          
          
            //allow ajax requests from browsers
            context.Response.Headers.Add("Access-Control-Allow-Origin","*");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE");
            
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