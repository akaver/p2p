using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public class RequestReceiveLedger
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context, string publicKey,
            string privateKey, object dbLock)
        {

            var bodyStr = "";
            var req = context.Request;

            // Allows using several time the stream in ASP.Net Core
            req.EnableRewind();

            // Arguments: Stream, Encoding, detect encoding, buffer size 
            // AND, the most important: keep stream opened
            using (StreamReader reader
                = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStr = reader.ReadToEnd();
            }

            // Rewind, so the core is not lost when it looks the body for the request
            req.Body.Position = 0;

            // new block, add it at end
            Console.WriteLine("Got json from body:" + bodyStr);

            return await Task.FromResult("{\"OK\": \"DONE\"}");
        }
    }
}