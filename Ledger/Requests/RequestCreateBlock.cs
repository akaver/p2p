using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public static class RequestCreateBlock
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context, string publicKey,
            string privateKey, object dbLock)
        {
            if (!context.Request.Query.ContainsKey("content") ||
                string.IsNullOrWhiteSpace(context.Request.Query["content"]))
                return await Task.FromResult("{\"ERROR\": \"NO CONTENT\"}");
            
            
            
            var content = context.Request.Query["content"];
            var childBlockJson = "";

            lock (dbLock)
            {

                // get the last block from db?
                var parentBlock = dbContext.Blocks.Single(b => b.ChildBlockId == null);

                var childBlock = new Block();
                childBlock.ParentBlockId = parentBlock.BlockId;

                // payload
                childBlock.CreatedAt = DateTime.Now;
                childBlock.Originator = publicKey;
                childBlock.Content = content;

                // payload signature
                childBlock.Signature = childBlock.GetPayloadSignature(privateKey);

                childBlock.LocalCreatedAt = childBlock.CreatedAt;
                childBlock.BlockId = childBlock.GetHash();

                parentBlock.ChildBlockId = childBlock.BlockId;

                dbContext.Blocks.Add(childBlock);

                var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
                childBlockJson = JsonConvert.SerializeObject(childBlock, settings);

                dbContext.SaveChanges();
            }

            return await Task.FromResult(childBlockJson);

        }
    }
}