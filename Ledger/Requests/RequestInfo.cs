using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public class RequestInfo
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context)
        {
            // get the last block in chain, send its hash - since all the hashes depend of each other, no need to recalculate

            var lastBlock = await dbContext.Blocks.SingleOrDefaultAsync(b => b.ChildBlockId == null);
            if (lastBlock != null)
            {
                var count = await dbContext.Blocks.CountAsync();
                return JsonConvert.SerializeObject(new {hash = lastBlock.BlockId, blockCount = count});
            }

            return await Task.FromResult("null");
        }
    }
}