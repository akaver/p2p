﻿using System;
using System.IO;
using Crypto;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using WebApp.Helpers;

namespace WebApp
{
    public class Program
    {
        public static int PortToListen = 5000;
        public static string InitialKnownHosts = "../Data/5000.json";
        public static string PublicKey = Guid.NewGuid().ToString();
        public static string PrivateKey = Guid.NewGuid().ToString();

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
                if (File.Exists(args[1]))
                {
                    InitialKnownHosts = args[1];
                }
                else
                {
                    Console.WriteLine("No data file found!!! From: " + InitialKnownHosts);
                }
            }

            // try to load private/public keys
            if (File.Exists(InitialKnownHosts))
            {
                using (var file = File.OpenText(InitialKnownHosts))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    var settings = (HostSettings) serializer.Deserialize(file, typeof(HostSettings));

                    var areKeysGenerated = !string.IsNullOrEmpty(settings.PublicKey) &&
                                           !string.IsNullOrEmpty(settings.PrivateKey);

                    if (!areKeysGenerated)
                    {
                        var keyPair = KeyGenerator.GetPrivatePublicKeyPair();
                        settings.PublicKey = keyPair.PublicKey;
                        settings.PrivateKey = keyPair.PrivateKey;

                        var newSettingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
                        File.WriteAllText(InitialKnownHosts, newSettingsJson);
                    }
                    
                    PublicKey = settings.PublicKey;
                    PrivateKey = settings.PrivateKey;
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