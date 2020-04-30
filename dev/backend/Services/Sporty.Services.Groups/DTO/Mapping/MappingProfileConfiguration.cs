using System.Runtime.CompilerServices;
using AutoMapper;
using Sporty.Common.Dto.Events.User;
using Sporty.Services.Groups.DTO.Model;
using Sporty.Services.Groups.DTO.Request;
using Sporty.Services.Groups.DTO.Response;

[assembly: InternalsVisibleTo("Sporty.Services.Groups.Tests")]
namespace Sporty.Services.Groups.DTO.Mapping
{
    internal class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<Group, CreateGroupRequest>().ReverseMap();
            CreateMap<Group, UpdateGroupRequest>().ReverseMap();
            CreateMap<Group, GroupQueryResponse>().ReverseMap();

            CreateMap<Member, UpdateGroupMembersRequest>().ReverseMap();
            CreateMap<Member, MemberQueryResponse>().ReverseMap();
            CreateMap<Member, UserCreatedEvent>().ReverseMap();
        }
    }
}
