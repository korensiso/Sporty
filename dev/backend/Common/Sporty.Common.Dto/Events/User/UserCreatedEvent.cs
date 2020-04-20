using System;
using Sporty.Infra.Data.Accessor.RabbitMQ.Events;

namespace Sporty.Common.Dto.Events.User
{
    public class UserCreatedEvent : IntegrationEvent
    {
        public Guid Identifier { get; }
        public string FirstName { get; }
        public string LastName { get; }

        public UserCreatedEvent(Guid identifier, string firstName, string lastName)
        {
            Identifier = identifier;
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString()
        {
            return $"Identifier={Identifier}, Name={FirstName} {LastName}";
        }
    }
}
