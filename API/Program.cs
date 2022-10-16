// <copyright company="">
// Copyright (c)  All rights reserved.
// </copyright>

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Cloud.$ext_safeprojectname$.API;
using Cloud.$ext_safeprojectname$.Domain.Logging;
using Serilog;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloud.$ext_safeprojectname$.API.Infrastructure.AutofacModules;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Extensions;
using System.IO;
using Microsoft.OpenApi;
using System.Diagnostics;

public class Program
{
    static readonly string DatabaseSecretStoreLocation = Environment.GetEnvironmentVariable("secret-location");
    public static void Main(string[] args)
    {
        try
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Console.WriteLine("Starting up");
            Log.Logger = LogConfigurator
                .ConfigureLogger()
                .CreateLogger();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .AddKeyPerFile($"{DatabaseSecretStoreLocation}", true, true)
                .Build();

            var app = BuildWebHost(args, configuration);
            
            app.ConfigureWebApplication(Log.Logger);

            app.Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Application start-up failed: {ex.Message}");
        }
    }

    public static WebApplication BuildWebHost(string[] args, IConfiguration configuration)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        builder.Host.ConfigureContainer<ContainerBuilder>((hostbuilder, containerBuilder) =>
            containerBuilder.RegisterModule(new ApplicationModule()));
        builder.Host.ConfigureContainer<ContainerBuilder>((hostbuilder, containerBuilder) =>
            containerBuilder.RegisterModule(new MediatorModule()));

        builder.Configuration.AddConfiguration(configuration);

        WebApplication app = builder.ConfigureServices(configuration)
            .Build();
        return app;
    }   
}
