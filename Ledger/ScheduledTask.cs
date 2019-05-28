using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DAL;
using Domain;
using Ledger.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Scheduler;

namespace Ledger
{
    public partial class ScheduledTask : IScheduledTask
    {
        private readonly IServiceProvider _serviceProvider;
        public TimeSpan TimeBetweenExecutions => TimeSpan.FromMinutes(1);
        private static readonly HttpClient HttpClient = new HttpClient();
        private static DateTime LastRunTime = DateTime.MinValue;

        public ScheduledTask(IServiceProvider serviceProvider)
        {
            HttpClient.Timeout = TimeSpan.FromSeconds(20);
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // update datetime
            LastRunTime = DateTime.Now;
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            Console.WriteLine("Scheduled task run at " + DateTime.Now);


            using (var scope = _serviceProvider.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var log = provider.GetRequiredService<Logger.IAppLogger>();
                var options = provider.GetRequiredService<IOptions<LedgerOptions>>().Value;

                
                using (var dbContext = provider.GetRequiredService<AppDbContext>())
                {
                    //Console.WriteLine("Generating new block");
                    // causes db to fail, blocks will branch due to timing
                    //await BlockHelper.GenerateNewBlockAsync(dbContext, log, options);
                    
                    
                    var tasks = new List<Task<HttpResponseMessage>>();
                    
                    
                    // ======================== ask for other hosts ============================
                    // do not ask from yourself
                    foreach (var host in dbContext.Hosts.Where(h => (h.Addr + h.Port) != (options.Addr + options.Port)))
                    {
                        var url = $"http://{host.Addr}:{host.Port}/ledger/addr?addr={options.Addr}&port={options.Port}";
                        Console.WriteLine($"Requesting from {url}");
                        tasks.Add(HttpClient.GetAsync(url, cancellationToken));   
                    }

                    
                    // this is doubtful!!!
                    // ======================== send out our new records ============================
                    /*
                    foreach (var blockToSend in dbContext.Blocks.Where(b => b.LocalCreatedAt >= (LastRunTime.Subtract(TimeBetweenExecutions).Subtract(TimeBetweenExecutions))))
                    {
                        foreach (var host in dbContext.Hosts.Where(h => (h.Addr + h.Port) != (options.Addr + options.Port)))
                        {
                            var url = $"http://{host.Addr}:{host.Port}/ledger/receiveblock?addr={options.Addr}&port={options.Port}&hash={blockToSend.Signature}";
                            Console.WriteLine($"Requesting from {url}");
                            tasks.Add(HttpClient.PostAsync(url, new StringContent(blockToSend.ToJson()), cancellationToken));   
                        }                           
                    }
                    */
                    
                    
                    // wait for all tasks to complete
                    await Task.WhenAll(tasks);

                    foreach (var task in tasks)
                    {
                        if (!task.Result.IsSuccessStatusCode)
                        {
                            continue;
                        }

                        if (task.Result.RequestMessage.RequestUri.ToString().Contains("/ledger/addr?addr="))
                        {
                            var requestHost = await dbContext.Hosts.SingleOrDefaultAsync(h =>
                                (h.Addr + h.Port) == (task.Result.RequestMessage.RequestUri.Host + task.Result.RequestMessage.RequestUri.Port), cancellationToken: cancellationToken);
                            if (requestHost != null)
                            {
                                requestHost.LastSeenDT = DateTime.Now;
                                dbContext.Hosts.Update(requestHost);
                                await dbContext.SaveChangesAsync(cancellationToken);
                                Console.WriteLine($"Got response from {task.Result.RequestMessage.RequestUri.Host}:{task.Result.RequestMessage.RequestUri.Port}, updated timestamp!");
                            }

                        
                        
                            var hosts = JsonConvert.DeserializeObject<List<Host>>(await task.Result.Content.ReadAsStringAsync());

                            foreach (var host in hosts)
                            {
                                if (await dbContext.Hosts.AsNoTracking().AnyAsync(h => h.Addr == host.Addr && h.Port == host.Port,
                                    cancellationToken: cancellationToken)) continue;
                                // got new host
                                Console.WriteLine($"New host found! {host.Addr}:{host.Port}");
                                host.LastSeenDT = null;
                                host.HostId = 0;
                                await dbContext.Hosts.AddAsync(host, cancellationToken);
                            }

                            await dbContext.SaveChangesAsync(cancellationToken);
                        }

                        if (task.Result.RequestMessage.RequestUri.ToString().Contains("/ledger/receiveblock?addr="))
                        {
                            var response = await task.Result.Content.ReadAsStringAsync();
                            Console.WriteLine("receiveblock response: " + response);
                        }


                    }
                    
                    Console.WriteLine("Active hosts: " + await dbContext.Hosts.Where(h => h.LastSeenDT != null).CountAsync(cancellationToken: cancellationToken));
                    
                    
                    // synchronize current blockchain with some other host
                    await SynchronizeLedger(dbContext, cancellationToken);

                    //using dbContext END
                }
            }

            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Scheduled task took " + stopWatch.Elapsed);


        }

        private async Task SynchronizeLedger(AppDbContext dbContext, CancellationToken cancellationToken){
            var localLedgerLength = await dbContext.Blocks.CountAsync();
            var localLastBlock = await dbContext.Blocks.SingleOrDefaultAsync(b => b.ChildBlockId == null);
            var localLastBlockHash = "";
            if (localLastBlock == null) {
                Console.WriteLine("Local ledger is empty!");
            }
            else
            {
                localLastBlockHash = localLastBlock.BlockId;
            }

            
            // choose host for syncing - first one where our ledgers dont match
            foreach (var host in await dbContext.Hosts.Where(h => h.LastSeenDT != null).ToListAsync())
            {
                Console.WriteLine($"Synchronizing ledger with host {host.Addr}:{host.Port}");
                // ask for merkle root (or its analog)
                var url = $"http://{host.Addr}:{host.Port}/ledger/info";
                var result = await HttpClient.GetAsync(url, cancellationToken);
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();
                    var remoteLedgerInfo = JsonConvert.DeserializeObject<LedgerInfo>(response);
                    Console.WriteLine("Response " + response);
                    if (localLastBlockHash != remoteLedgerInfo.Hash)
                    {
                        Console.WriteLine("Local and remote ledgers don't match. Syncing....");
                        // get complete remote ledger
                        url = $"http://{host.Addr}:{host.Port}/ledger/blocks";
                        var resultLedger = await HttpClient.GetAsync(url, cancellationToken);
                        if (resultLedger.IsSuccessStatusCode)
                        {
                            var  resultLedgerContent = await resultLedger.Content.ReadAsStringAsync();
                            // deserialize
                            var remoteLedgerBlocks = JsonConvert.DeserializeObject<List<LedgerBlock>>(resultLedgerContent);
                            // iterate over items, add new ones into our ledger
                            foreach (var ledgerBlock in remoteLedgerBlocks)
                            {
                                Console.Write("Block Id: " + ledgerBlock.BlockId + " - ");
                                // check, if we already have this block or no
                                var weHaveThisBlock =
                                    await dbContext.Blocks.AnyAsync(b => b.Signature == ledgerBlock.Signature, cancellationToken);
                                if (!weHaveThisBlock)
                                {
                                    // new block for us, insert it at end!
                                    Console.WriteLine("new block!");
                                        
                                    var lastBlock = await dbContext.Blocks.SingleAsync(b => b.ChildBlockId == null, cancellationToken);
                                    
                                    
                                    var block = new Block();
                                    block.ParentBlockId = localLastBlock?.BlockId;
                                    block.ChildBlockId = null;

                                    // payload
                                    block.CreatedAt = ledgerBlock.CreatedAt;
                                    block.Originator = ledgerBlock.Originator;
                                    // this should be separate request, can be huge in theory - rest is just metadata
                                    block.Content = ledgerBlock.Content;

                                    // payload signature
                                    block.Signature = ledgerBlock.Signature;

                                    block.LocalCreatedAt = DateTime.Now;
                                    block.BlockId = block.GetHash();

                                    lastBlock.ChildBlockId = block.BlockId;

                                    await dbContext.Blocks.AddAsync(block);
                                    await dbContext.SaveChangesAsync();                                    
                                    Console.WriteLine("Inserted!");
                                    
                                }
                                else
                                {
                                    Console.WriteLine("already have it!");
                                }

                            }
                            // push our new complete ledger back - we now have our original ledger and new blocks from other host
                            url = $"http://{host.Addr}:{host.Port}/ledger/receiveledger";
                            var blocksToSend = await dbContext.Blocks.ToListAsync(cancellationToken: cancellationToken);
                            var contentToSend = "[";
                            foreach (var block in blocksToSend)
                            {
                                contentToSend = contentToSend + block.ToJson() + ",";
                            }

                            contentToSend = contentToSend.TrimEnd(',');
                            
                            contentToSend = contentToSend + "]";
                            var postResult = await HttpClient.PostAsync(url, new StringContent(contentToSend), cancellationToken);
                            if (postResult.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Sent ledger over!");
                            }
                            else
                            {
                                Console.WriteLine("Failure in sync! " + postResult.ReasonPhrase);
                            }

                        }
                        break; // sync only single host in every run
                    }
                }
                
            } // foreach
            
        }
    }


}