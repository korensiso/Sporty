using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;

namespace Sporty.Infra.Data.Accessor.RabbitMQ.Implementations
{
    public class EventsBusConfiguration : IEventsBusConfiguration
    {
        public int EventBusRetryCount { get; set; }
        public string EventBusConnection { get; set; }

        public override string ToString()
        {
            return $"EventBusConnection={EventBusConnection}, EventBusRetryCount={EventBusRetryCount}";
        }
    }
}
