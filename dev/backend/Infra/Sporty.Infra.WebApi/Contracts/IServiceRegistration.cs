using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sporty.Infra.WebApi.Contracts
{
    //NOTE: registrations are via reflection, so it seems like the implementations are not in use
    public interface IServiceRegistration
    {
        void RegisterAppServices(IServiceCollection services, IConfiguration configuration);
    }
}
