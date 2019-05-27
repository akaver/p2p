using System;
using System.Collections;
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
    public class RequestReceiveBlock
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context, string publicKey,
            string privateKey, object dbLock)
        {
            if (!context.Request.Query.ContainsKey("hash") ||
                string.IsNullOrWhiteSpace(context.Request.Query["hash"]))
                return await Task.FromResult("{\"ERROR\": \"NO HASH\"}");


            // do we already have this content
            var hash = context.Request.Query["hash"];
            if (await dbContext.Blocks.AnyAsync(b => b.Signature == hash))
            {
                Console.WriteLine("ALREADY HAVE");
                return await Task.FromResult("{\"OK\": \"ALREADY HAVE\"}");
            }

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


            var receivedBlock = JsonConvert.DeserializeObject<Block>(bodyStr);
            
            // verify it one more time
            if (await dbContext.Blocks.AnyAsync(b => b.Signature == receivedBlock.Signature))
            {
                Console.WriteLine("ALREADY HAVE");
                return await Task.FromResult("{\"OK\": \"ALREADY HAVE\"}");
            }
            
            

            lock (dbLock)
            {

                // get the last block from db?
                var lastBlock = dbContext.Blocks.Single(b => b.ChildBlockId == null);

                receivedBlock.ParentBlockId = lastBlock.BlockId;


                receivedBlock.LocalCreatedAt = DateTime.Now;
                receivedBlock.BlockId = receivedBlock.GetHash();

                lastBlock.ChildBlockId = receivedBlock.BlockId;

                receivedBlock.ChildBlockId = null;

                dbContext.Blocks.Add(receivedBlock);

                dbContext.SaveChanges();
            }

            return await Task.FromResult("{\"OK\": \"BLOCK ADDED\"}");
        }
    }
}