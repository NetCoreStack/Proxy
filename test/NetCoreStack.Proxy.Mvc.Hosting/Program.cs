using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace NetCoreStack.Proxy.Mvc.Hosting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
       .UseKestrel()
       .UseContentRoot(Directory.GetCurrentDirectory())
       .ConfigureAppConfiguration((hostingContext, config) =>
       {
           var env = hostingContext.HostingEnvironment;
           config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
           config.AddEnvironmentVariables();
       })
       .ConfigureLogging((hostingContext, logging) =>
       {
           logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
           logging.AddConsole();
           logging.AddDebug();
       })
       .UseStartup<Startup>()
       .Build();

            webHost.Run();
        }
    }
}
