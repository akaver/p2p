﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApp
{
    public class Program
    {
        public static int PortToListen = 5000;
        public static string InitialKnownHosts = "/Users/akaver/Magister/VR2/P2P/Data/5000.json";

        public static void Main(string[] args)
        {
            Console.WriteLine("Usage: dotnet run WebApp.dll <portno> <hosts.json>");

            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out var portFromArgs))
                {
                    PortToListen = portFromArgs;
                }
            }

            if (args.Length > 1)
            {
                if (!File.Exists(args[1]))
                {
                    InitialKnownHosts = args[1];
                }
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://0.0.0.0:{PortToListen}/");
    }
}