using Cloud.$ext_safeprojectname$.Events;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Cloud.$ext_safeprojectname$.API
{
    /// <summary>
    /// Register your services here.
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        private static string ServiceName => Environment.GetEnvironmentVariable("SERVICE_NAME");
        private static string Namespace => Environment.GetEnvironmentVariable("NAMESPACE");
        private static IWebHostEnvironment _env;
        public const string EventAssemblyName = "Cloud.$ext_safeprojectname$.Events";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            IServiceCollection services = builder.Services;
            _env = builder.Environment;
            services.ConfigureServices(configuration);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {            
            var eventAssemblyTypes = Assembly.Load(EventAssemblyName)            
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
                        
            _ = services.ConfigureControllers()
                .ConfigureHttp(configuration)
                .AddDbContext(configuration, _env)                
                .AddIntegrations(configuration, eventAssemblyTypes, EventAssemblyName)
                .AddHealthChecks();

            _ = services.AddTransient(s =>
                s.GetService<IHttpContextAccessor>()?.HttpContext?.User);
        }

        private static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            // Adding the assembly explicitly allows us to use a Startup file in different assembly.
            string swaggerConfig = Environment.GetEnvironmentVariable("disableSwagger") ?? "false";

            if (bool.TryParse(swaggerConfig, out bool disableSwagger) && !disableSwagger)
            {
                _ = services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = ServiceName, Version = "v1" });                    
                    c.EnableAnnotations();
                    if (!File.Exists(Path.Combine(System.AppContext.BaseDirectory,
                            $"{typeof(ServiceConfiguration).Assembly.GetName().Name}.xml"))) return;
                    var filePath = Path.Combine(System.AppContext.BaseDirectory,
                        $"{typeof(ServiceConfiguration).Assembly.GetName().Name}.xml");
                    c.IncludeXmlComments(filePath);
                });
            }

            Assembly appAssembly = Assembly.GetAssembly(typeof(ServiceConfiguration));

            services
                .AddControllers()
                .AddApplicationPart(appAssembly)
                .AddControllersAsServices();
            services.AddMvc(_ =>
            {
                //configure mvc options here. e.g model binding behaviour
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            });

            return services;
        }


        private static IServiceCollection ConfigureHttp(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Information("Adding http helpers");
            string allowedOrigins = configuration.GetValue<string>("AllowedHosts") ?? "*";
            CorsPolicy corsPolicy = new CorsPolicyBuilder().WithOrigins(allowedOrigins)
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod().Build();
            // HttpContext

            _ = services
                .AddHttpContextAccessor()
                .AddHttpClient()
                .AddCors(options =>
                {
                    options.AddPolicy("Default", corsPolicy);
                });
            return services;
        }
    }
}