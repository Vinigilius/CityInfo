using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace CityInfo.API
{
    /// <summary>
    /// This is the base class for runnig application. 
    /// To test all features I used Postman, to simplyfy writing http requests and test functionality.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try {
                logger.Error("init main");
                BuildWebHost(args).Run();
            } catch (Exception ex) {
                //NLog catches setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            } finally {
                // Ensure to flush and stop internal timers/threads before application-exit. We have to clean all up.
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information); // We are going to log all information at minimum information level.
                })
                .UseNLog() // Starting using nlog to log informations to the files.
                .Build();
    }
}
