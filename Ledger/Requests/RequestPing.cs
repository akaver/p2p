using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public class RequestPing
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context)
        {
            return "OK";
        }

    }
}