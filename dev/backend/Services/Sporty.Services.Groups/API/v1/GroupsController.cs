using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sporty.Common.Dto.Group.Model;
using Sporty.Common.Dto.Group.Request;
using Sporty.Common.Dto.Group.Response;
using Sporty.Common.Network.Http.QueryUrl;
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
        private readonly IMapper _mapper;
        public GroupsController(IGroupManager groupManager, IMapper mapper, ILogger<GroupsController> logger)
        {
            _groupManager = groupManager;
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

        [Route("{id:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(GroupQueryResponse), Status200OK)]
        [ProducesResponseType(typeof(GroupQueryResponse), Status404NotFound)]
        public async Task<GroupQueryResponse> Get(Guid id)
        {
            Group group = await _groupManager.GetByIdAsync(id);
            return group != null ? _mapper.Map<GroupQueryResponse>(group)
                                  : throw new ApiProblemDetailsException($"Record with id: {id} does not exist.", Status404NotFound);
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

        [Route("{id:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Put(Guid id, [FromBody] UpdateGroupRequest updateRequest)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            Group group = _mapper.Map<Group>(updateRequest);
            group.Identifier = id;

            if (await _groupManager.UpdateAsync(group))
            {
                return new ApiResponse($"Record with Id: {id} successfully updated.", true);
            }

            throw new ApiProblemDetailsException($"Record with Id: {id} does not exist.", Status404NotFound);
        }

        [Route("{id:Guid/users}")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Put(Guid id, [FromBody] UpdateGroupUsersRequest updateGroupUsersRequest)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            if (await _groupManager.UpdateGroupUsersAsync(id, updateGroupUsersRequest.Users))
            {
                return new ApiResponse($"Record with Id: {id} successfully updated.", true);
            }

            throw new ApiProblemDetailsException($"Record with Id: {id} does not exist.", Status404NotFound);
        }


        [Route("{id:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            if (await _groupManager.DeleteAsync(id))
            {
                return new ApiResponse($"Record with Id: {id} successfully deleted.", true);
            }
            else
            {
                throw new ApiProblemDetailsException($"Record with id: {id} does not exist.", Status404NotFound);
            }
        }
    }
}