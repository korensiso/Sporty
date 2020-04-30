using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sporty.Common.Dto.Events.User;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;
using Sporty.Services.Users.DTO.Model;
using Sporty.Services.Users.DTO.Request;
using Sporty.Services.Users.DTO.Response;
using Sporty.Services.Users.Manager;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Sporty.Services.Users.API.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public UsersController(IUserManager userManager, IMapper mapper, IEventBus eventBus, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _eventBus = eventBus;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserQueryResponse>), Status200OK)]
        public async Task<IEnumerable<UserQueryResponse>> Get()
        {
            IEnumerable<User> data = await _userManager.GetAllAsync();
            IEnumerable<UserQueryResponse> users = _mapper.Map<IEnumerable<UserQueryResponse>>(data);

            return users;
        }

        [Route("paged")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserQueryResponse>), Status200OK)]
        public async Task<IEnumerable<UserQueryResponse>> Get([FromQuery] PaginationQuery paginationQuery)
        {
            (IEnumerable<User> Users, Pagination Pagination) data = await _userManager.GetUsersAsync(paginationQuery);
            IEnumerable<UserQueryResponse> users = _mapper.Map<IEnumerable<UserQueryResponse>>(data.Users);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(data.Pagination));

            return users;
        }

        [Route("{id:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(UserQueryResponse), Status200OK)]
        [ProducesResponseType(typeof(UserQueryResponse), Status404NotFound)]
        public async Task<UserQueryResponse> Get(Guid id)
        {
            User user = await _userManager.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserQueryResponse>(user)
                                  : throw new ApiProblemDetailsException($"Record with id: {id} does not exist.", Status404NotFound);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Post([FromBody] CreateUserRequest createRequest)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            User user = _mapper.Map<User>(createRequest);
            Guid result = await _userManager.CreateAsync(user);
            user = await _userManager.GetByIdAsync(user.Identifier);

            UserCreatedEvent userCreatedEvent = _mapper.Map<UserCreatedEvent>(user);
            _eventBus.Publish(userCreatedEvent);

            return new ApiResponse("Record successfully created.", result, Status201Created);
        }

        [Route("{id:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Put(Guid id, [FromBody] UpdateUserRequest updateRequest)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            User user = _mapper.Map<User>(updateRequest);
            user.Identifier = id;

            if (await _userManager.UpdateAsync(user))
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
            if (await _userManager.DeleteAsync(id))
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