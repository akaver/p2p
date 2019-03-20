using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Ledger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scheduler;
using Tor;
using WebApp.Helpers;

namespace WebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("p2p"));
            services.AddScoped<Logger.IAppLogger, Logger.AppLogger>();

            services.Configure<LedgerOptions>(options =>
            {
                options.Addr = "127.0.0.1";
                options.Port = Program.PortToListen;
                options.Path = "/ledger";
            });
            
            // Add scheduled tasks & scheduler
            services.AddSingleton<IScheduledTask, Ledger.ScheduledTask>();
            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // load the initial list of potentially known peers in network
            // check for config file - specified from command line as second parameter
            // get the db context from di container
            if (Program.InitialKnownHosts != null)
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var provider = scope.ServiceProvider;
                    var log = provider.GetRequiredService<Logger.IAppLogger>();
                    var options = provider.GetRequiredService<IOptions<LedgerOptions>>().Value;
                    using (var dbContext = provider.GetRequiredService<AppDbContext>())
                    {
                        LoadInitialSettings.LoadInitialSettingsFromJsonFile(dbContext, log, Program.InitialKnownHosts, options);                  
                    }
                }
            }
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            

            app.UseTor("/tor");
            app.UseLedger("/ledger");

            app.Run((context) => Task.CompletedTask);
            
            //app.Run(async (context) => { await context.Response.WriteAsync("OK"); });
        }
    }
}