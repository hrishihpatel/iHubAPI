using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace iHubAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .ReadFrom.Configuration(config)
               //.WriteTo.Console()
               //.WriteTo.File(new RenderedCompactJsonFormatter(), "/Users/hrishikeshpatel/Documents/GitHub/iHubAPI/log.ndjson")
               .CreateLogger();

            try
            {
                Log.Information("Starting application");
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
