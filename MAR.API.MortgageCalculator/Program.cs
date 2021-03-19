using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAR.API.MortgageCalculator
{
    /// <summary>
    /// The program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry points
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            try
            {
                var webHost = CreateHostBuilder(args).Build();
                using (var scope = webHost.Services.CreateScope())
                {
                    Log.Information("IP Policy store is being seeded...");
                    var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();
                    await ipPolicyStore.SeedAsync();
                }
                Log.Information("Application is starting up...");
                webHost.Run();
            }
            catch (Exception ex)
            {
                if (Log.Logger == null)
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();
                }
                Log.Fatal(ex, "Application failed to startup");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Host builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .CaptureStartupErrors(true)
                        .UseSerilog((hostingContext, loggerConfiguration) =>
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostingContext.Configuration)
                                .Enrich.FromLogContext()
                                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                                .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
#if DEBUG
                            loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
                        });
                });
    }
}
