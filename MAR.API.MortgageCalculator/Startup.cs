using AspNetCoreRateLimit;
using MAR.API.MortgageCalculator.Interfaces;
using MAR.API.MortgageCalculator.Logic.Facade;
using MAR.API.MortgageCalculator.Logic.Factories;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Logic.Providers;
using MAR.API.MortgageCalculator.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
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
            services.AddOptions();
            ConfigureAppSettings(services);
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            ConfigureLocalization(services);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MAR.API.MortgageCalculator", Version = "v1" });
                var xmlFilePath = Path.Combine(System.AppContext.BaseDirectory, "MAR.API.MortgageCalculator.xml");
                c.IncludeXmlComments(xmlFilePath);
            });
            AddRateLimitingServices(services);
            AddApiDomainServices(services);
        }

        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        }
        private void ConfigureLocalization(IServiceCollection services)
        {
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
        }

        private void AddApiDomainServices(IServiceCollection services)
        {
            services.AddScoped<IHttpClientProvider, HttpClientProvider>();
            services.AddScoped<IMortgageCalculatorProviderFactory, MortgageCalculatorProviderFactory>();
            services.AddScoped<IMortgageCalculatorFacade, MortgageCalculatorFacade>();
            services.AddSingleton<IAuthorizationProvider, AuthorizationProvider>();
        }

        private void AddRateLimitingServices(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="memoryCache"></param>
        /// <param name="appSettings"></param>
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IMemoryCache memoryCache, 
            IOptions<AppSettings> appSettings)
        {
            app.UseIpRateLimiting();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MAR.API.MortgageCalculator v1"));
            }
            
            app.UseRequestLocalization();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //setup memory cache users
            var neverExpireOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove);
            if (appSettings.Value.PublicPaidAccessUserId != Guid.Empty
                && !string.IsNullOrWhiteSpace(appSettings.Value.PublicPaidAccessUserPassword))
            {
                memoryCache.Set($"PAIDUSERS_CLIENTID_{appSettings.Value.PublicPaidAccessUserId}", appSettings.Value.PublicPaidAccessUserId, neverExpireOptions);
                memoryCache.Set($"PAIDUSERS_CLIENTID_{appSettings.Value.PublicPaidAccessUserId}_PASSWORD", appSettings.Value.PublicPaidAccessUserPassword, neverExpireOptions);
            }
        }
    }
}
