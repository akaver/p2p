using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public static class RequestAddr
    {
        public static async Task<string> Response(AppDbContext dbContext)
        {
            var jsonString = JsonConvert.SerializeObject(await dbContext.Hosts.ToListAsync(), Formatting.Indented);
            return jsonString;
        }
    }
}