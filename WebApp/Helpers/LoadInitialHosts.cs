using System;
using System.Collections.Generic;
using System.IO;
using DAL;
using Domain;
using Newtonsoft.Json;

namespace WebApp.Helpers
{
    public static class LoadInitialHosts
    {
        public static void LoadInitialHostsFromJsonFile(AppDbContext ctx, string jsonDataFile)
        {
            // validate inputs
            if (ctx == null) throw new ArgumentNullException(nameof(AppDbContext));
            if (string.IsNullOrWhiteSpace(jsonDataFile)) throw new ArgumentNullException(nameof(jsonDataFile));
            if (!File.Exists(jsonDataFile)) throw new FileNotFoundException(nameof(jsonDataFile), jsonDataFile);

            // import data in db

            using (StreamReader file = File.OpenText(jsonDataFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                var hosts = (List<Host>) serializer.Deserialize(file, typeof(List<Host>));
                foreach (var host in hosts)
                {
                    ctx.Hosts.Add(host);
                }
            }
            ctx.SaveChanges();
            
        }
    }
}