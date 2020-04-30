using System.Runtime.CompilerServices;
using AutoMapper;
using Sporty.Common.Dto.Events.User;
using Sporty.Services.Users.DTO.Model;
using Sporty.Services.Users.DTO.Request;
using Sporty.Services.Users.DTO.Response;

[assembly: InternalsVisibleTo("Sporty.Services.Users.Tests")]
namespace Sporty.Services.Users.DTO.Mapping
{
    internal class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<User, CreateUserRequest>().ReverseMap();
            CreateMap<User, UpdateUserRequest>().ReverseMap();
            CreateMap<User, UserQueryResponse>().ReverseMap();
            CreateMap<User, UserCreatedEvent>().ReverseMap();
        }
    }
}
