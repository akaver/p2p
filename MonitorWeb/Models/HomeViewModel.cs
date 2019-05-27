using System.Collections.Generic;

namespace MonitorWeb.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Endpoints = new List<EndpointData>();
        }
        
        public List<EndpointData> Endpoints { get; set; }
    }

    public class EndpointData
    {
        public string Url { get; set; }
        public int PortNumber { get; set; }
    }
}