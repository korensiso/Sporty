using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Sporty.Services.Users
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();
            var logger = builder.Services.GetService<ILogger<Program>>();
            try
            {
                logger.LogInformation($"Starting web host - {typeof(Program).Assembly.GetName().Name}");
                builder.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Host unexpectedly terminated");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureHostConfiguration(configBuilder =>
                    configBuilder
                        .AddJsonFile("appsettings.Logs.json", true, true)
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
               .UseSerilog((hostingContext, loggerConfig) =>
                    loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
                );
    }
}
