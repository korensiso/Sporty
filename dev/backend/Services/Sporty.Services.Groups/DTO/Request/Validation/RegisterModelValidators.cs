using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sporty.Common.Dto.Group.Request;
using Sporty.Infra.WebApi.Contracts;

namespace Sporty.Services.Groups.DTO.Request.Validation
{
    internal class RegisterModelValidators : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
        {
            //Register DTO Validators
            services.AddTransient<IValidator<CreateGroupRequest>, CreateGroupRequestValidator>();
            services.AddTransient<IValidator<UpdateGroupRequest>, UpdateGroupRequestValidator>();
            services.AddTransient<IValidator<UpdateGroupUsersRequest>, UpdateGroupUsersRequestValidator>();

            //Disable Automatic Model State Validation built-in to ASP.NET Core
            services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
        }
    }
}
