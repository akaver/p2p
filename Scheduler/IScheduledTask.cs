using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler
{
    public interface IScheduledTask
    {
        TimeSpan TimeBetweenExecutions  { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
    
    
}