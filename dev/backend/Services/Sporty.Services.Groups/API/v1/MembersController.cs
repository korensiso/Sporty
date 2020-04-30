using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sporty.Common.Network.Http.Headers;
using Sporty.Services.Groups.DTO.Model;
using Sporty.Services.Groups.DTO.Request;
using Sporty.Services.Groups.DTO.Response;
using Sporty.Services.Groups.Manager;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Sporty.Services.Groups.API.v1
{
    [Route("api/v1/Groups/{groupId:Guid}/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ILogger<MembersController> _logger;
        private readonly IGroupManager _groupManager;
        private readonly IMemberManager _memberManager;
        private readonly IMapper _mapper;

        public MembersController(IGroupManager groupManager, IMemberManager memberManager, IMapper mapper, ILogger<MembersController> logger)
        {
            _groupManager = groupManager;
            _memberManager = memberManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MemberQueryResponse>), Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<MemberQueryResponse>), Status404NotFound)]
        public async Task<IEnumerable<MemberQueryResponse>> Get(Guid groupId)
        {
            Group group = await _groupManager.GetByIdAsync(groupId);
            if (group == null)
            {
                throw new ApiProblemDetailsException($"Group with Id: {groupId} does not exist.",Status404NotFound);
            }

            var members = new List<MemberQueryResponse>(group.Members.Count);
            foreach (Guid memberId in group.Members)
            {
                Member member = await _memberManager.GetByIdAsync(memberId);
                _logger.LogError($"{member}");
                MemberQueryResponse memberQueryResponse = _mapper.Map<MemberQueryResponse>(member);
                _logger.LogError($"{memberQueryResponse}");
                members.Add(memberQueryResponse);  
            }

            return members;
        }

        [Route("{memberId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(MemberQueryResponse), Status200OK)]
        [ProducesResponseType(typeof(MemberQueryResponse), Status404NotFound)]
        public async Task<MemberQueryResponse> Get(Guid groupId, Guid memberId)
        {
            Group group = await _groupManager.GetByIdAsync(groupId);
            if (group == null)
            {
                throw new ApiProblemDetailsException($"Group with Id: {groupId} does not exist.", Status404NotFound);
            }

            if (!group.ContainsMember(memberId))
            {
                throw new ApiProblemDetailsException($"Member with Id: {memberId} does not belong to group with id: {groupId}.", Status404NotFound);
            }

            Member member = await _memberManager.GetByIdAsync(memberId);

            return member != null ? _mapper.Map<MemberQueryResponse>(member)
                : throw new ApiProblemDetailsException($"Member with id: {groupId} does not exist.", Status404NotFound);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status206PartialContent)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Put(Guid groupId, [FromBody] UpdateGroupMembersRequest updateGroupUsersRequest,
            [FromHeader(Name = Headers.Count)] int count)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            foreach (Guid memberId in updateGroupUsersRequest.Members)
            {
                if (!await _memberManager.ExistAsync(memberId))
                    throw new ApiProblemDetailsException($"Member with Id: {memberId} does not exist.", Status404NotFound);
            }

            (int groupUpdated, int memberUpdated) updateResult = await _groupManager.AddGroupMembersAsync(groupId, updateGroupUsersRequest.Members);
            if (updateResult.memberUpdated > 0)
            {
                if (updateResult.memberUpdated < count)
                {
                    return new ApiResponse($"Group with Id: {groupId} updated partially.", updateResult, Status206PartialContent);
                }

                if (updateResult.memberUpdated == count)
                {
                    return new ApiResponse($"Group with Id: {groupId} updated partially.", updateResult);
                }
            }

            throw new ApiProblemDetailsException($"Group with Id: {groupId} does not exist. Or some of the users does not exist", Status404NotFound);
        }

        [Route("{memberId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Put(Guid groupId, Guid memberId)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            if (!await _memberManager.ExistAsync(memberId))
            {
                throw new ApiProblemDetailsException($"Member with Id: {memberId} does not exist.", Status404NotFound);
            }

            (int groupUpdated, int memberUpdated) updateResult = await _groupManager.AddGroupMembersAsync(groupId, new []{memberId});
            if (updateResult.memberUpdated == 1)
            {
                return new ApiResponse($"Member with Id: {memberId} successfully added to group with id: {groupId}.", memberId);
            }

            throw new ApiProblemDetailsException($"Group with id: {memberId} does not exist.", Status404NotFound);
        }

        [Route("{memberId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        public async Task<ApiResponse> Delete(Guid groupId, Guid memberId)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            if (!await _memberManager.ExistAsync(memberId))
            {
                throw new ApiProblemDetailsException($"Member with Id: {memberId} does not exist.", Status404NotFound);
            }

            (int groupUpdated, int memberUpdated) updateResult = await _groupManager.DeleteGroupMembersAsync(groupId, new[] { memberId });
            if (updateResult.memberUpdated == 1)
            {
                return new ApiResponse($"Member with Id: {memberId} successfully deleted from group with id: {groupId}.", memberId);
            }

            throw new ApiProblemDetailsException($"Group with id: {memberId} does not exist.", Status404NotFound);
        }
    }
}
