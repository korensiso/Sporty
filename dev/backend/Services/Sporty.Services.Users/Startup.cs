using AspNetCoreRateLimit;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AutoWrapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Sporty.Infra.Data.Accessor.Mongo.Models;
using Sporty.Infra.Data.Accessor.RabbitMQ.Extensions;
using Sporty.Infra.Data.Accessor.RabbitMQ.Implementations;
using Sporty.Services.Users.Bootstrap;
using Sporty.Services.Users.DTO.Mapping;

namespace Sporty.Services.Users
{
    internal class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; set; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
            if (Environment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddOptions();
            services.Configure<MongoConfiguration>(Configuration.GetSection(nameof(MongoConfiguration)));
            services.Configure<EventsBusConfiguration>(Configuration.GetSection(nameof(EventsBusConfiguration)));

            //Register services in Installers folder
            services.AddServicesInAssembly(Configuration);

            //Register MVC/Web API, NewtonsoftJson and add FluentValidation Support
            services.AddControllers()
                    .AddNewtonsoftJson(ops => { ops.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; })
                    .AddFluentValidation(fv => { fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false; });

            //Register Automapper
            services.AddAutoMapper(typeof(MappingProfileConfiguration));

            services.AddEventBusConnection(Configuration)
                    .AddEventBus(Configuration);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like: event handlers
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //Enable Swagger and SwaggerUI
            app.UseSwagger();
            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.DocumentTitle = Configuration["ServiceName"] + ":Swagger";
                swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Configuration["ServiceName"]} ASP.NET Core API v1");
            });

            //Enable HealthChecks and UI
            //app.UseHealthChecks("/selfcheck", new HealthCheckOptions
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //}).UseHealthChecksUI(setup =>
            //{
            //    setup.AddCustomStylesheet($"{env.ContentRootPath}/Infrastructure/HealthChecks/Ux/branding.css");
            //});

            //Enable AutoWrapper.Core
            //More info see: https://github.com/proudmonkey/AutoWrapper
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions { IsDebug = true, UseApiProblemDetailsException = true });

            //Enable AspNetCoreRateLimit
            app.UseIpRateLimiting();

            app.UseRouting();

            //Enable CORS
            app.UseCors("AllowAll");

            //Adds authenticaton middleware to the pipeline so authentication will be performed automatically on each request to host
            app.UseAuthentication();

            //Adds authorization middleware to the pipeline to make sure the Api endpoint cannot be accessed by anonymous clients
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
