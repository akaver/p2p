using System;

namespace Domain
{
    public class Host
    {
        public int HostId { get; set; }
        public string Addr { get; set; }
        public int Port { get; set; }
        public DateTime? LastSeenDT { get; set; } = null;
        
    }
    
}