namespace Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces
{
    interface IEventsBusConfiguration
    {
        int EventBusRetryCount { get; set; }
        string EventBusConnection { get; set; }
    }
}
