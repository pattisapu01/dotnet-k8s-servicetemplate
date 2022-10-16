using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using Serilog.Enrichers.Span;
using System.IO;

namespace Cloud.$ext_safeprojectname$.Domain.Logging
{
    public class LogConfigurator
    {
        public static LoggerConfiguration ConfigureLogger()
        {
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
              .AddEnvironmentVariables()
              .Build();

            return new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                 .Enrich.When((le) => le.Properties.ContainsKey("scope") ? true : false, (e) => e.WithProperty("scope", "{scope}"))
                 .Enrich.WithProperty("ServiceName", configuration["SERVICE_NAME"])
                 .Enrich.WithSpan()
                 .Enrich.FromLogContext()
                 .Enrich.With<EventTypeEnricher>()
                 .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
                 .ReadFrom.Configuration(configuration);
        }
    }
}