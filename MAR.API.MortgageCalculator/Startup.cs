using MAR.API.MortgageCalculator.Logic.Facade;
using MAR.API.MortgageCalculator.Logic.Factories;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Logic.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MAR.API.MortgageCalculator
{
    /// <summary>
    /// The startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Instance
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>()
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("en-GB"),
                    };
                    options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MAR.API.MortgageCalculator", Version = "v1" });
                var xmlFilePath = Path.Combine(System.AppContext.BaseDirectory, "MAR.API.MortgageCalculator.xml");
                c.IncludeXmlComments(xmlFilePath);
            });
            services.AddScoped<IHttpClientProvider, HttpClientProvider>();
            services.AddScoped<IMortgageCalculatorProviderFactory, MortgageCalculatorProviderFactory>();
            services.AddScoped<IMortgageCalculatorFacade, MortgageCalculatorFacade>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MAR.API.MortgageCalculator v1"));
            }

            app.UseRequestLocalization();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
