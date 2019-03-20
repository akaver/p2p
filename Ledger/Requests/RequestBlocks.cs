using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public class RequestBlocks
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

            var responseBlocks = new List<Block>();
            
            var block =  await dbContext.Blocks.FirstOrDefaultAsync(b => b.ParentBlockId == null);

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
                } while (block?.ParentBlockId != null);
            }


            return JsonConvert.SerializeObject(responseBlocks);
            
        }
    }
}