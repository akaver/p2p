using System;
using System.Threading.Tasks;

namespace Logger
{
    public interface IAppLogger
    {
        Task VerboseAsync(string category, string entry);
        Task DebugAsync(string category, string entry);
        Task InfoAsync(string category, string entry);
        Task WarnAsync(string category, string entry);
        Task ErrorAsync(string category, string entry);
        Task WTFAsync(string category, string entry);
    }
    
}