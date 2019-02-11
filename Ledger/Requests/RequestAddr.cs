using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ledger.Requests
{
    public static class RequestAddr
    {
        public static async Task<string> Response(AppDbContext dbContext, HttpContext context)
        {
            //if there is a requester info in query, add it to known hosts (if not already there)
            if (
                context.Request.Query.ContainsKey("addr") &&
                context.Request.Query.ContainsKey("port"))
            {
                if (int.TryParse(context.Request.Query["port"], out var port))
                {
                    // check, that its not already in db
                    if (!await dbContext.Hosts.AnyAsync(h => h.Addr == context.Request.Query["addr"] && h.Port == port))
                    {
                        await dbContext.Hosts.AddAsync(new Host()
                        {
                            HostId = 0,
                            Addr = context.Request.Query["addr"],
                            Port = port,
                            LastSeenDT = null
                        });
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        /*
                        var host = await dbContext.Hosts.SingleAsync(h =>
                            h.Addr == context.Request.Query["addr"] && h.Port == port);
                        host.LastSeenDT = DateTime.Now;
                        dbContext.Hosts.Update(host);
                        await dbContext.SaveChangesAsync();
                        */
                    }

                }
            }


            // do not include yourself? id1
            var jsonString = JsonConvert.SerializeObject(await dbContext.Hosts.Where(h => h.HostId != 1).ToListAsync(), Formatting.Indented);
            return jsonString;
        }
    }
}