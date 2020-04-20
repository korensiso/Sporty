using System.Threading.Tasks;

namespace Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}