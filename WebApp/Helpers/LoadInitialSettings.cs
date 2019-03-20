using System;
using System.Collections.Generic;
using System.IO;
using DAL;
using Domain;
using Ledger;
using Logger;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebApp.Helpers
{
    public static class LoadInitialSettings
    {
        public static async void LoadInitialSettingsFromJsonFile(AppDbContext ctx, Logger.IAppLogger log,
            string jsonDataFile, LedgerOptions options)
        {
            // validate inputs
            if (ctx == null) throw new ArgumentNullException(nameof(AppDbContext));
            if (string.IsNullOrWhiteSpace(jsonDataFile)) throw new ArgumentNullException(nameof(jsonDataFile));
            if (!File.Exists(jsonDataFile)) throw new FileNotFoundException(nameof(jsonDataFile), jsonDataFile);

            // import data into db, get public and private key

            // add ourselves as first in list
            await ctx.Hosts.AddAsync(new Host()
            {
                Addr = options.Addr,
                Port = options.Port,
                LastSeenDT = DateTime.Now
            });


            using (var file = File.OpenText(jsonDataFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                
                var settings = (HostSettings) serializer.Deserialize(file, typeof(HostSettings));

                Program.PublicKey = settings.PublicKey;
                Program.PrivateKey = settings.PrivateKey;
                
                foreach (var host in settings.Hosts)
                {
                    await ctx.Hosts.AddAsync(host);
                }
            }

            await ctx.SaveChangesAsync();

            await log.InfoAsync(
                "startup - Keys",
                $"Public: {Program.PublicKey} Private: {Program.PrivateKey}"
            );

            await log.InfoAsync(
                "startup - import hosts",
                JsonConvert.SerializeObject(await ctx.Hosts.ToListAsync(), Formatting.None)
            );
            
            // generate GENESIS block and start to synchronize with others
            
            var genesisBlock = new Block();
            genesisBlock.ParentBlockId = null;
            genesisBlock.ChildBlockId = null;

            // payload
            genesisBlock.CreatedAt = DateTime.Now;
            genesisBlock.Originator = Program.PublicKey;
            genesisBlock.Content = "GENESIS BLOCK";

            // payload signature
            genesisBlock.Signature = genesisBlock.GetPayloadSignature(Program.PublicKey);

            genesisBlock.LocalCreatedAt = genesisBlock.CreatedAt;
            genesisBlock.BlockId = genesisBlock.GetHash();


            await ctx.Blocks.AddAsync(genesisBlock);
            await ctx.SaveChangesAsync();

            await log.InfoAsync(
                "startup - created GENESIS block",
                JsonConvert.SerializeObject(await ctx.Blocks.FirstOrDefaultAsync(), Formatting.None)
            );

        }
    }
}