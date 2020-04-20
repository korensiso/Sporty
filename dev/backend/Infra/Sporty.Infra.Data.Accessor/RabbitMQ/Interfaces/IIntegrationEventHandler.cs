using System.Threading.Tasks;
using Sporty.Infra.Data.Accessor.RabbitMQ.Events;

namespace Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces
{
    public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler
        where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
