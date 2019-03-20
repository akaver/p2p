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
    public class ScheduledTask : IScheduledTask
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

                    
                    // ======================== send out our new records ============================
                    foreach (var blockToSend in dbContext.Blocks.Where(b => b.LocalCreatedAt >= LastRunTime))
                    {
                        foreach (var host in dbContext.Hosts.Where(h => (h.Addr + h.Port) != (options.Addr + options.Port)))
                        {
                            var url = $"http://{host.Addr}:{host.Port}/ledger/receiveblock?addr={options.Addr}&port={options.Port}&hash={blockToSend.Signature}";
                            Console.WriteLine($"Requesting from {url}");
                            tasks.Add(HttpClient.PostAsync(url, new StringContent(blockToSend.ToJson()), cancellationToken));   
                        }                           
                    }
                    
                    
                    
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
                }
            }

            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Scheduled task took " + stopWatch.Elapsed);


        }
    }
}