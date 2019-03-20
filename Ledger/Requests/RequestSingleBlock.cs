using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public class RequestSingleBlock
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context)
        {
            //if there is a requester info in query, add it to known hosts (if not already there)
            if (context.Request.Query.ContainsKey("hash"))
            {
                var hash = context.Request.Query["hash"];

                var block = await dbContext.Blocks.FirstOrDefaultAsync(b => b.BlockId == hash);

                return JsonConvert.SerializeObject(block);
            }

            return await Task.FromResult("null");
        }
        
    }
}