using System;
using RabbitMQ.Client;

namespace Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
