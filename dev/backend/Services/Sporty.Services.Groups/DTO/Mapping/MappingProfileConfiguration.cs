using System.Runtime.CompilerServices;
using AutoMapper;
using Sporty.Common.Dto.Group.Model;
using Sporty.Common.Dto.Group.Request;
using Sporty.Common.Dto.Group.Response;

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
        }
    }
}
