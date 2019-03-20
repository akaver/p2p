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

                var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
                return JsonConvert.SerializeObject(block,settings);
            }

            if (context.Request.Query.ContainsKey("payloadhash"))
            {
                var payloadhash = context.Request.Query["payloadhash"];

                var block = await dbContext.Blocks.FirstOrDefaultAsync(b => b.Signature == payloadhash);

                var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
                return JsonConvert.SerializeObject(block,settings);
            }

            

            return await Task.FromResult("null");
        }
        
    }
}