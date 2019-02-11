using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DAL;
using Domain;
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
        private static HttpClient _httpClient = new HttpClient();

        public ScheduledTask(IServiceProvider serviceProvider)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
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
                    var tasks = new List<Task<HttpResponseMessage>>();
                    // do not ask from yourself
                    foreach (var host in dbContext.Hosts.Where(h => !(h.Addr == options.Addr && h.Port == options.Port)))
                    {
                        var url = $"http://{host.Addr}:{host.Port}/ledger/addr?addr={options.Addr}&port={options.Port}";
                        Console.WriteLine($"Requesting from {url}");
                        tasks.Add(_httpClient.GetAsync(url));   
                    }

                    // wait for all tasks to complete
                    await Task.WhenAll(tasks);


                    foreach (var task in tasks)
                    {
                        if (!task.Result.IsSuccessStatusCode)
                        {
                            continue;
                        }

                        
                        
                            
                        var requestHost = await dbContext.Hosts.SingleAsync(h =>
                            h.Addr == task.Result.RequestMessage.RequestUri.Host && h.Port == task.Result.RequestMessage.RequestUri.Port);
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
                    
                    Console.WriteLine("Active hosts: " + await dbContext.Hosts.Where(h => h.LastSeenDT != null).CountAsync(cancellationToken: cancellationToken));
                }
            }

            stopWatch.Stop();
            Console.WriteLine("Scheduled task took " + stopWatch.Elapsed);


        }
    }
}