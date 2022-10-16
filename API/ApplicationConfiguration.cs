using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Prometheus;
using Serilog;

namespace Cloud.$ext_safeprojectname$.API
{
    /// <summary>
    /// Api configuration.
    /// </summary>
    public static class ApplicationConfiguration
{
    /// <summary>
    /// Configures http pipeline i.e it's all middleware until terminated. 
    /// </summary>
    /// <param name="app"></param>
    public static void ConfigureWebApplication(this IApplicationBuilder app, ILogger logger)
    {
        Log.Information("http/s pipeline");
        string swaggerConfig = Environment.GetEnvironmentVariable("disableSwagger") ?? "false";
        if (bool.TryParse(swaggerConfig, out bool disableSwagger) && !disableSwagger)
        {
            logger.Information("Swagger Enabled!");
            app.UseSwagger();

            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "$ext_safeprojectname$ v1");
                swaggerUiOptions.RoutePrefix = string.Empty;
            });
        }
        app.UseExceptionHandler("/Error");
        app.UseRouting();
        app.UseHttpMetrics(opt =>
        {
            opt.InProgress.Enabled = true;
            opt.RequestCount.Enabled = true;
            opt.CaptureMetricsUrl = true;
        });
        app.UseCors("Default");
        

        app.UseSerilogRequestLogging();
        app.UseAuthorization();
        app.UseAuthentication();        
        app.UseStaticFiles();
        app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapGet("/ping", async context =>
                {
                    await context.Response.WriteAsync("Ok");
                });
                endpoints.MapControllers();
            });
    }
}
}
