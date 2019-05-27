using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public class RequestLedgerHash
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context)
        {
            // get the last block in chain, send its hash - since all the hashes 
            return await Task.FromResult("null");
        }
    }
}