using System;
using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;

namespace Sporty.Infra.Data.Accessor.RabbitMQ.Implementations
{
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        public class SubscriptionInfo
        {
            public bool IsDynamic { get; }
            public Type HandlerType { get; }

            private SubscriptionInfo(bool isDynamic, Type handlerType)
            {
                IsDynamic = isDynamic;
                HandlerType = handlerType;
            }

            public static SubscriptionInfo Dynamic(Type handlerType)
            {
                return new SubscriptionInfo(true, handlerType);
            }
            public static SubscriptionInfo Typed(Type handlerType)
            {
                return new SubscriptionInfo(false, handlerType);
            }

            public override string ToString()
            {
                return $"IsDynamic={IsDynamic}, HandlerType={HandlerType}";
            }
        }
    }
}
