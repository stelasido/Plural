using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Plural
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run(); // RUN APP AND LISTEN
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(SetupConfiguration)
                .UseStartup<Startup>() // WHAT CLASS TO USE TO SET UP HOW  TO LISTEN FOR REQUESTS
                .Build(); // BUILD

        private static void SetupConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
        {
            // REMOVING THE DEFAULT CONFIGURATION OPTIONS
            builder.Sources.Clear();
            builder.AddJsonFile("config.json", false, true)
                .AddEnvironmentVariables();
        }
    }
}
