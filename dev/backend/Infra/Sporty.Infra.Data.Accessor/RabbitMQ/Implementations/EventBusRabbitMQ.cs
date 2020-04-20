using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Sporty.Infra.Data.Accessor.RabbitMQ.Events;
using Sporty.Infra.Data.Accessor.RabbitMQ.Extensions;
using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;

namespace Sporty.Infra.Data.Accessor.RabbitMQ.Implementations
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private const string c_eventBusContextName = "sporty_event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autoFac;
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autoFac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _autoFac = autoFac;
            _retryCount = retryCount;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: c_eventBusContextName,
                    routingKey: eventName);

                if (!_subsManager.IsEmpty) return;

                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning($"Could not publish event: {@event.Id} after {_retryCount} attempts ({ex.Message})", ex);
                    //_logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            string eventName = @event.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using (IModel channel = _persistentConnection.CreateModel())
            {

                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

                channel.ExchangeDeclare(exchange: c_eventBusContextName, type: "direct");

                string message = JsonConvert.SerializeObject(@event);
                byte[] body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    if (channel != null)
                    {
                        IBasicProperties properties = channel.CreateBasicProperties();
                        properties.DeliveryMode = 2; // persistent

                        _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                        channel.BasicPublish(
                            exchange: c_eventBusContextName,
                            routingKey: eventName,
                            mandatory: true,
                            basicProperties: properties,
                            body: body);
                    }
                });
            }
        }

        public void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", 
                eventName, 
                typeof(TH).GetGenericTypeName());

            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
            StartBasicConsume();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            string eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event EventName={EventName} EventHandler={EventHandler}", eventName, typeof(TH).GetGenericTypeName());

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
            
            _logger.LogInformation("Subscribing is done! EventName={EventName} EventHandler={EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                                      exchange: c_eventBusContextName,
                                      routingKey: eventName);
                }
            }
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();

            _subsManager.Clear();
        }

        private void StartBasicConsume()
        {
            _logger.LogInformation("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            string eventName = eventArgs.RoutingKey;
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            _logger.LogInformation($"Event received name={eventName}, message={message}");

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"----- ERROR Processing message \"{message}\"", ex);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            IModel channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: c_eventBusContextName, type: "direct");

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, callbackExceptionEventArgs) =>
            {
                _logger.LogWarning("Recreating RabbitMQ consumer channel", callbackExceptionEventArgs.Exception);

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogInformation($"Processing RabbitMQ event: eventName={eventName} message={message}");

            try
            {
                if (_subsManager.HasSubscriptionsForEvent(eventName))
                {
                    using (var scope = _autoFac.BeginLifetimeScope(c_eventBusContextName))
                    {
                        IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> subscriptions = _subsManager.GetHandlersForEvent(eventName);

                        foreach (var subscription in subscriptions)
                        {
                            if (subscription.IsDynamic)
                            {
                                IDynamicIntegrationEventHandler handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                                if (handler == null) continue;
                                dynamic eventData = JObject.Parse(message);

                                await Task.Yield();
                                await handler.Handle(eventData);
                            }
                            else
                            {
                                //_subsManager.GetHandlersForEvent<>()
                                object handler = scope.ResolveOptional(subscription.HandlerType);
                                if (handler == null) continue;

                                Type eventType = _subsManager.GetEventTypeByName(eventName);
                                object integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                                Type concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                                await Task.Yield();
                                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not process event. Exception={e}");
                throw;
            }
        }
    }
}
