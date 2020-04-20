using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sporty.Common.Dto.Events.User;
using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;

namespace Sporty.Services.Groups.Events.Handlers
{
    internal class UserCreatedEventHandler : IIntegrationEventHandler<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(UserCreatedEvent @event)
        {
            _logger.LogInformation($"Handling event {@event}");

            await Task.CompletedTask;
        }
    }
}
