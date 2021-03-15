using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                    webBuilder.UseStartup<Startup>();
                });
    }
}