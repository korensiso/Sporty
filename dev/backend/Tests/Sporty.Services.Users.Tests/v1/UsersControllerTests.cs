using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.Extensions.Logging;
using Moq;
using Sporty.Common.Dto.Events.User;
using Sporty.Infra.Data.Accessor.RabbitMQ.Interfaces;
using Sporty.Services.Users.API.v1;
using Sporty.Services.Users.DTO.Mapping;
using Sporty.Services.Users.DTO.Model;
using Sporty.Services.Users.DTO.Request;
using Sporty.Services.Users.DTO.Response;
using Sporty.Services.Users.Manager;
using Sporty.Tests.Utils;
using Xunit;

namespace Sporty.Services.Users.Tests.v1
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserManager> _userManager;
        private readonly Mock<IEventBus> _eventBus;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            ILogger<UsersController> logger = Mock.Of<ILogger<UsersController>>();

            var mapperProfile = new MappingProfileConfiguration();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            var mapper = new Mapper(configuration);

            _userManager = new Mock<IUserManager>();
            _eventBus = new Mock<IEventBus>();

            _controller = new UsersController(_userManager.Object, mapper, _eventBus.Object, logger);
        }

        private static IEnumerable<User> GetFakeUserLists()
        {
            return new List<User>
            {
                new User()
                {
                    Identifier = GuidConstants.Guid1,
                    FirstName = "Vynn Markus",
                    LastName = "Durano",
                    DateOfBirth = Convert.ToDateTime("01/15/2016")
                },
                new User()
                {
                    Identifier = GuidConstants.Guid2,
                    FirstName = "Vianne Maverich",
                    LastName = "Durano",
                    DateOfBirth = Convert.ToDateTime("02/15/2016")
                }
            };
        }

        private static CreateUserRequest FakeCreateRequestObject()
        {
            return new CreateUserRequest()
            {
                FirstName = "Vinz",
                LastName = "Durano",
                DateOfBirth = Convert.ToDateTime("02/15/2016")
            };
        }

        private static UpdateUserRequest FakeUpdateRequestObject()
        {
            return new UpdateUserRequest()
            {
                FirstName = "Vinz",
                LastName = "Durano",
                DateOfBirth = Convert.ToDateTime("02/15/2016")
            };
        }

        private static CreateUserRequest FakeCreateRequestObjectWithMissingAttribute()
        {
            return new CreateUserRequest()
            {
                FirstName = "Vinz",
                LastName = "Durano"
            };
        }

        [Fact]
        public async Task Get_All_Ok()
        {

            // Arrange
            _userManager.Setup(manager => manager.GetAllAsync())
               .ReturnsAsync(GetFakeUserLists());

            // Act
            IEnumerable<UserQueryResponse> result = await _controller.Get();

            // Assert
            List<UserQueryResponse> users = Assert.IsType<List<UserQueryResponse>>(result);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task Get_ById_Ok()
        {
            Guid id = GuidConstants.Guid1;

            _userManager.Setup(manager => manager.GetByIdAsync(id))
               .ReturnsAsync(GetFakeUserLists().Single(p => p.Identifier.Equals(id)));

            UserQueryResponse userQueryResponse = await _controller.Get(id);
            Assert.IsType<UserQueryResponse>(userQueryResponse);
        }

        [Fact]
        public async Task Get_ById_NotFound()
        {
            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Get(GuidConstants.Guid4));
            Assert.Equal(404, apiException.StatusCode);
        }

        [Fact]
        public async Task Post_DateOfBirthMissing_BadRequest()
        {
            _controller.ModelState.AddModelError("DateOfBirth", "Required");

            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Post(FakeCreateRequestObjectWithMissingAttribute()));
            Assert.Equal(422, apiException.StatusCode);
        }

        [Fact]
        public async Task Post_UserNotExists_Ok()
        {
            CreateUserRequest fakeCreateRequestObject = FakeCreateRequestObject();
            _userManager.Setup(manager => manager.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(It.IsAny<Guid>());
            _userManager.Setup(manager => manager.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User 
                    { 
                        Identifier = Guid.NewGuid(), 
                        FirstName = fakeCreateRequestObject.FirstName, 
                        LastName = FakeCreateRequestObject().LastName
                    });

            ApiResponse response = await _controller.Post(fakeCreateRequestObject);

            ApiResponse apiResponse = Assert.IsType<ApiResponse>(response);
            Assert.Equal(201, apiResponse.StatusCode);
            _eventBus.Verify(mock => mock.Publish(It.IsAny<UserCreatedEvent>()));
        }

        [Fact]
        public async Task Post_UserManagerThrows_Throws()
        {
            _userManager.Setup(manager => manager.CreateAsync(It.IsAny<User>()))
                .Throws(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _controller.Post(FakeCreateRequestObject()));
        }

        [Fact]
        public async Task Put_ById_Ok()
        {
            _userManager.Setup(manager => manager.UpdateAsync(It.IsAny<User>()))
                 .ReturnsAsync(true);

            ApiResponse user = await _controller.Put(GuidConstants.Guid1, FakeUpdateRequestObject());

            ApiResponse response = Assert.IsType<ApiResponse>(user);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task Put_UserNotExists_NotFound()
        {
            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Put(GuidConstants.Guid4, FakeUpdateRequestObject()));
            Assert.Equal(404, apiException.StatusCode);
        }

        [Fact]
        public async Task Put_DateOfBirthMissing_BadRequest()
        {
            _controller.ModelState.AddModelError("DateOfBirth", "Required");

            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Put(GuidConstants.Guid4, FakeUpdateRequestObject()));
            Assert.Equal(422, apiException.StatusCode);
        }

        [Fact]
        public async Task Put_UserManagerThrows_Throws()
        {

            _userManager.Setup(manager => manager.UpdateAsync(It.IsAny<User>()))
                .Throws(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _controller.Put(GuidConstants.Guid4, FakeUpdateRequestObject()));
        }

        [Fact]
        public async Task Delete_ById_Ok()
        {
            Guid id = GuidConstants.Guid1;

            _userManager.Setup(manager => manager.DeleteAsync(id))
                 .ReturnsAsync(true);

            var result = await _controller.Delete(id);

            var response = Assert.IsType<ApiResponse>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task Delete_UserNotExists_NotFound()
        {
            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Delete(GuidConstants.Guid1));
            Assert.Equal(404, apiException.StatusCode);
        }

        [Fact]
        public async Task Delete_UserManagerThrows_Throws()
        {
            Guid id = GuidConstants.Guid1;

            _userManager.Setup(manager => manager.DeleteAsync(id))
                .Throws(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _controller.Delete(id));
        }
    }
}
