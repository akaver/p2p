using System.Threading.Tasks;
using DAL;
using Domain;

namespace Logger
{
    public class AppLogger : IAppLogger
    {
        private readonly AppDbContext _ctx;

        public AppLogger(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task VerboseAsync(string category, string entry)
        {
             await AddLogEntryAsync("Verbose", category, entry);
        }

        public async Task DebugAsync(string category, string entry)
        {
            await AddLogEntryAsync("Verbose", category, entry);
        }

        public async Task InfoAsync(string category, string entry)
        {
            await AddLogEntryAsync("Verbose", category, entry);
        }

        public async Task WarnAsync(string category, string entry)
        {
            await AddLogEntryAsync("Verbose", category, entry);
        }

        public async Task ErrorAsync(string category, string entry)
        {
            await AddLogEntryAsync("Verbose", category, entry);
        }

        public async Task WTFAsync(string category, string entry)
        {
            await AddLogEntryAsync("Verbose", category, entry);
        }

        private async Task AddLogEntryAsync(string level, string category, string entry)
        {
            await _ctx.LogEntries.AddAsync(new LogEntry()
            {
                Level = level,
                Category = category,
                Entry = entry
            });
            await _ctx.SaveChangesAsync();
        }
    }
}