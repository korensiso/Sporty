﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sporty.Infra.WebApi.Contracts;

namespace Sporty.Services.Users.Bootstrap
{
    internal static class ServiceRegistrationExtension
    {
        public static void AddServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var appServices = new List<IServiceRegistration>();
            var assemblies = new List<Assembly> {typeof(Startup).Assembly};
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => asm.FullName.Contains("sporty.infra", StringComparison.InvariantCultureIgnoreCase)));

            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<IServiceRegistration> appServicesInAssembly = assembly.DefinedTypes
                    .Where(x => typeof(IServiceRegistration)
                    .IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IServiceRegistration>();

                appServices.AddRange(appServicesInAssembly);
            }

            appServices.ForEach(svc =>
            {
                svc.RegisterAppServices(services, configuration);
            });
        }
    }
}
