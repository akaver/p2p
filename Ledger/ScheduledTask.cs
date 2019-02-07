using System;
using System.Threading;
using System.Threading.Tasks;
using Scheduler;

namespace Ledger
{
    public class ScheduledTask : IScheduledTask
    {
        public TimeSpan TimeBetweenExecutions => TimeSpan.FromMinutes(1);
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Scheduled task run at " + DateTime.Now);
            await Task.Delay(5000, cancellationToken);
            
        }
    }
}