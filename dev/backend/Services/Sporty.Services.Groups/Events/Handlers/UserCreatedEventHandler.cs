using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Sporty.Common.Dto.Events.User;
using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;
using Sporty.Services.Groups.DTO.Model;
using Sporty.Services.Groups.Manager;

namespace Sporty.Services.Groups.Events.Handlers
{
    internal class UserCreatedEventHandler : IIntegrationEventHandler<UserCreatedEvent>
    {
        private readonly IMemberManager _memberManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(IMemberManager memberManager, IMapper mapper, ILogger<UserCreatedEventHandler> logger)
        {
            _memberManager = memberManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(UserCreatedEvent @event)
        {
            _logger.LogInformation($"Handling event {@event}");
            Member member = _mapper.Map<Member>(@event);

            await _memberManager.CreateAsync(member);
        }
    }
}
