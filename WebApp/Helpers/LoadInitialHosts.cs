using System;
using System.Collections.Generic;
using System.IO;
using DAL;
using Domain;
using Logger;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebApp.Helpers
{
    public static class LoadInitialHosts
    {
        public async static void LoadInitialHostsFromJsonFile(AppDbContext ctx, Logger.IAppLogger log, string jsonDataFile)
        {
            // validate inputs
            if (ctx == null) throw new ArgumentNullException(nameof(AppDbContext));
            if (string.IsNullOrWhiteSpace(jsonDataFile)) throw new ArgumentNullException(nameof(jsonDataFile));
            if (!File.Exists(jsonDataFile)) throw new FileNotFoundException(nameof(jsonDataFile), jsonDataFile);

            // import data in db

            using (var file = File.OpenText(jsonDataFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                var hosts = (List<Host>) serializer.Deserialize(file, typeof(List<Host>));
                foreach (var host in hosts)
                {
                    await ctx.Hosts.AddAsync(host);
                }
            }
            await ctx.SaveChangesAsync();

            await log.InfoAsync(
                "startup - import hosts",
                JsonConvert.SerializeObject(await ctx.Hosts.ToListAsync(), Formatting.None)
                );

        }
    }
}