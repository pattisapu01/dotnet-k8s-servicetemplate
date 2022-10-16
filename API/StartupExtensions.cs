using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Domain.Interfaces;
using Cloud.$ext_safeprojectname$.Infrastructure.DataContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;

namespace Cloud.$ext_safeprojectname$.API
{
    public static class StartupExtensionMethods
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<$ext_safeprojectname$DataContext>(ServiceLifetime.Scoped);

            return services;
        }
        public static IServiceCollection AddIntegrations(this IServiceCollection services, IConfiguration configuration, List<Type> eventAssemblyTypes, string eventAssemblyName)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IConnectionBuilder, ConnectionBuilder>()
                .AddScoped<ISecretReader, SecretReader>();

            _ = services.ConfigureApiBehaviour();

            //tye based resources (ONLY for local debugging)
            MigrateDatabase(services, configuration);
            return services;
        }
        
        public static IServiceCollection ConfigureApiBehaviour(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });
            return services;
        }
        
        public static IServiceCollection MigrateDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var dbConnString = configuration.GetConnectionString("db-store");
            
            if (string.IsNullOrEmpty(dbConnString))
                return services;
            var d = dbConnString.ChunkStringIntoKeyValuePairs(Convert.ToChar(";"), Convert.ToChar("="));
            StringBuilder sb = new StringBuilder();
            //we are in "tye" land
            //Invoke the "flyway command line tool" and migrations
            var flywayPath = Path.Combine(Path.Combine(Environment.CurrentDirectory, $"{configuration["flywayfolder"]}"), "flyway.cmd");
            if (!System.IO.File.Exists(flywayPath))
            {
                Console.WriteLine($"Downloading flyway command line from https://repo1.maven.org/maven2/org/flywaydb/flyway-commandline/{configuration["flywayversion"]}/flyway-commandline-{configuration["flywayversion"]}-windows-x64.zip");
                //download zip and extract
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile($"https://repo1.maven.org/maven2/org/flywaydb/flyway-commandline/{configuration["flywayversion"]}/flyway-commandline-{configuration["flywayversion"]}-windows-x64.zip", Path.Combine(Environment.CurrentDirectory, "flyway.zip"));
                }
                if (System.IO.File.Exists(Path.Combine(Environment.CurrentDirectory, "flyway.zip")))
                {
                    Console.WriteLine("Un-zipping flyway package..");
                    ZipFile.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory, "flyway.zip"), Environment.CurrentDirectory);
                    Console.WriteLine("Flyway package extracted..");
                }
                if (!System.IO.File.Exists(flywayPath))
                {
                    throw new Exception($"Flyway command line tool not found at {flywayPath}");
                }
            }
            var sqlPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).FullName, @"migrations\sql");


            
            bool isDBAvailable = false;
            while (!isDBAvailable)
            {
                if (String.IsNullOrEmpty(configuration["secret-key-prefix"]))
                    throw new Exception("Please define a secret-key-prefix = service name in appsettings file");
                isDBAvailable = IsDBAvailable($"Server=localhost;Port=5438;User Id={d["User Id"]};Password={d["Password"]};Database={d["Database"]}");
                if (!isDBAvailable)
                {
                    Console.WriteLine($"DB {configuration["secret-key-prefix"]} is not yet up on localhost:5438! re-trying...");
                    Thread.Sleep(1000);
                }
            }
            
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = flywayPath,
                    Arguments = $"migrate -X -url=\"jdbc:postgresql://localhost:5438/{d["Database"]}\" -schemas=public -user={d["User Id"]} -password={d["Password"]} -locations=filesystem:/\"{sqlPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            using (proc)
            {
                if (proc.Start())
                {
                    while (!proc.HasExited)
                    {
                        sb.Append(proc.StandardOutput.ReadToEnd() + Environment.NewLine);
                    }
                    Console.WriteLine(sb.ToString());
                }
            }


            return services;
        }

        private static bool IsDBAvailable(string connStr)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(connStr);
                conn.Open();
                conn.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static Dictionary<string, string> ChunkStringIntoKeyValuePairs(this string stringToChunk, char firstDelimeter, char keyValueDelimiter)
        {
            var result = new Dictionary<string, string>();
            var a = stringToChunk.Split(Convert.ToChar(firstDelimeter));
            for (var i = 0; i <= a.Length - 1; i++)
            {
                var b = a[i].Split(Convert.ToChar(keyValueDelimiter));
                if (b.Length <= 1)
                    continue;
                result.Add(b[0], b[1]);
            }
            return result;
        }
    }
}
