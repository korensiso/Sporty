using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sporty.Common.Network.Http.Headers;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Services.Groups.DTO.Model;
using Sporty.Services.Groups.DTO.Request;
using Sporty.Services.Groups.DTO.Response;
using Sporty.Services.Groups.Manager;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Sporty.Services.Groups.API.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly IGroupManager _groupManager;
        private readonly IMemberManager _memberManager;
        private readonly IMapper _mapper;

        public GroupsController(IGroupManager groupManager, IMemberManager memberManager, IMapper mapper, ILogger<GroupsController> logger)
        {
            _groupManager = groupManager;
            _memberManager = memberManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GroupQueryResponse>), Status200OK)]
        public async Task<IEnumerable<GroupQueryResponse>> Get()
        {
            IEnumerable<Group> data = await _groupManager.GetAllAsync();
            IEnumerable<GroupQueryResponse> groups = _mapper.Map<IEnumerable<GroupQueryResponse>>(data);

            return groups;
        }

        [Route("paged")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GroupQueryResponse>), Status200OK)]
        public async Task<IEnumerable<GroupQueryResponse>> Get([FromQuery] PaginationQuery paginationQuery)
        {
            (IEnumerable<Group> Groups, Pagination Pagination) data = await _groupManager.GetGroupsAsync(paginationQuery);
            IEnumerable<GroupQueryResponse> groups = _mapper.Map<IEnumerable<GroupQueryResponse>>(data.Groups);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(data.Pagination));

            return groups;
        }

        [Route("{groupId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(GroupQueryResponse), Status200OK)]
        [ProducesResponseType(typeof(GroupQueryResponse), Status404NotFound)]
        public async Task<GroupQueryResponse> Get(Guid groupId, [FromQuery]GroupsQueryUrl groupsQuery)
        {
            Group group = await _groupManager.GetByIdAsync(groupId);
            if (!groupsQuery.IncludeMembers) group.Members.Clear();

            return group != null ? _mapper.Map<GroupQueryResponse>(group)
                                  : throw new ApiProblemDetailsException($"Record with groupId: {groupId} does not exist.", Status404NotFound);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Post([FromBody] CreateGroupRequest createRequest)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            Group group = _mapper.Map<Group>(createRequest);
            return new ApiResponse("Record successfully created.", await _groupManager.CreateAsync(group), Status201Created);
        }

        [Route("{groupId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Put(Guid groupId, [FromBody] UpdateGroupRequest updateRequest)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            Group group = _mapper.Map<Group>(updateRequest);
            group.Identifier = groupId;

            if (await _groupManager.UpdateAsync(group))
            {
                return new ApiResponse($"Record with Id: {groupId} successfully updated.", true);
            }

            throw new ApiProblemDetailsException($"Record with Id: {groupId} does not exist.", Status404NotFound);
        }

        [Route("{groupId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        public async Task<ApiResponse> Delete(Guid groupId)
        {
            if (await _groupManager.DeleteAsync(groupId))
            {
                return new ApiResponse($"Record with Id: {groupId} successfully deleted.", true);
            }

            throw new ApiProblemDetailsException($"Record with groupId: {groupId} does not exist.", Status404NotFound);
        }
    }
}