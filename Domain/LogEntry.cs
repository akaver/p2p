using System;

namespace Domain
{
    public class LogEntry
    {
        public int LogEntryId { get; set; }
        
        
        public DateTime DT { get; set; } = DateTime.Now;
        public string Level { get; set; }
        public string Category { get; set; }
        public string Entry { get; set; }
    }
}