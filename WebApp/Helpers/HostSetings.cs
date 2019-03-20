using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApp.Helpers
{
    public class HostSettings
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public List<Host> Hosts { get; set; } = new List<Host>();
    }

   
}