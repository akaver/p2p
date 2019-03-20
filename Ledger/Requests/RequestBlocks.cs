using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public static class RequestBlocks
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context)
        {
            string fromHash = null;
            string toHash = null;
            if (context.Request.Query.ContainsKey("from"))
            {
                fromHash = context.Request.Query["from"];
            }

            if (context.Request.Query.ContainsKey("to"))
            {
                toHash = context.Request.Query["to"];
            }

            if (context.Request.Query.ContainsKey("all"))
            {
                if (context.Request.Query["all"] == "true")
                {
                    var res = await dbContext.Blocks.OrderBy(b => b.LocalCreatedAt).ToListAsync();
                    var res2 = res.Select(a => new {block = a, child = a.ChildBlockId});
                    return JsonConvert.SerializeObject(res2, new JsonSerializerSettings {Formatting = Formatting.Indented});
                }
            }
            
            
            
            var responseBlocks = new List<Block>();

            Block block = null;
            if (fromHash == null)
            {
                // get the first block
                block =  await dbContext.Blocks.FirstOrDefaultAsync(b => b.ParentBlockId == null);
            }
            else
            {
                // start from the correct block
                block =  await dbContext.Blocks.FirstOrDefaultAsync(b => b.BlockId == fromHash);
            }
            

            if (block != null)
            {
                responseBlocks.Add(block);
                do
                {
                    //get the block, where previous block is parent
                    block = await dbContext.Blocks.FirstOrDefaultAsync(b => b.ParentBlockId == block.BlockId);
                    if (block != null)
                    {
                        responseBlocks.Add(block);
                    }

                    if (toHash!=null && block != null && block.BlockId == toHash) break;
                    
                } while (block?.ParentBlockId != null);
            }

            var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
            return JsonConvert.SerializeObject(responseBlocks, settings);
            
        }
    }
}