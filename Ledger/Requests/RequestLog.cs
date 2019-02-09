using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public static class RequestLog
    {
        public static async Task<string> Response(AppDbContext dbContext)
        {
            var jsonString = JsonConvert.SerializeObject(await dbContext.LogEntries.OrderByDescending(l => l.DT).Take(20).ToListAsync(), Formatting.Indented);
            return jsonString;
        }
    }
}