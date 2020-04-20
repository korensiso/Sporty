using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sporty.Infra.Data.Accessor.RabbitMQ.Implementations;
using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;

namespace Sporty.Infra.Data.Accessor.RabbitMQ.Extensions
{
    public static class EventBusRegistration
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            string subscriptionClientName = configuration["ServiceName"];

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(serviceProvider =>
            {
                ILifetimeScope iLifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
                IRabbitMQPersistentConnection rabbitMQPersistentConnection = serviceProvider.GetRequiredService<IRabbitMQPersistentConnection>();
                ILogger<EventBusRabbitMQ> logger = serviceProvider.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                IEventBusSubscriptionsManager eventBusSubscriptionsManager = serviceProvider.GetRequiredService<IEventBusSubscriptionsManager>();

                int retryCount = 5;
                string configuredRetryCount = configuration["EventsBusConfiguration:EventBusRetryCount"];
                if (!string.IsNullOrEmpty(configuredRetryCount))
                {
                    retryCount = int.Parse(configuredRetryCount);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IServiceCollection AddEventBusConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(serviceProvider =>
            {
                ILogger<RabbitMQPersistentConnection> logger = serviceProvider.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventsBusConfiguration:EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventsBusConfiguration:EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventsBusConfiguration:EventBusRetryCount"]);
                }

                return new RabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }
    }
}
