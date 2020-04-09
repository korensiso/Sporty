using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sporty.Common.Dto.User.Model;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;
using Sporty.Infra.Data.Accessor.Mongo.Models;
using Sporty.Infra.Data.Accessor.Mongo.Repository;

namespace Sporty.Services.Users.Manager
{
    internal class UserManager : IUserManager
    {
        private readonly ILogger<UserManager> _logger;
        private readonly IMongoConfiguration _mongoConfiguration;
        private readonly MongoRepository<User, Guid> _usersRepo;

        public UserManager(IConfiguration config, ILogger<UserManager> logger)
        {
            _logger = logger;

            string connectionString = string.Format(config.GetConnectionString("SQLDBConnectionString"), "localhost", "27017");
            _mongoConfiguration = new MongoConfiguration(connectionString, "sporty");
            _usersRepo = new MongoRepository<User, Guid>(_mongoConfiguration); 
            _usersRepo.Init("Users");
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _usersRepo.GetAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _usersRepo.FirstOrDefaultAsync(user => user.Identifier == id);
        }

        public async Task<Guid> CreateAsync(User entity)
        {
            entity.Identifier = Guid.NewGuid();
            entity.CreatedOn = DateTime.UtcNow;

            await _usersRepo.AddAsync(entity);
            
            return entity.Identifier;
        }

        public async Task<bool> UpdateAsync(User entity)
        {
            User currentUser = await GetByIdAsync(entity.Identifier);
            if (currentUser == null) return false;

            long modified = await _usersRepo.UpdateAsync(user => user.Identifier == entity.Identifier, 
                currentUser.Update(entity));
            
            return modified != 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            long deleted = await _usersRepo.DeleteAsync(user => user.Identifier == id);
            return deleted != 0;
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            return await GetByIdAsync(id) != null;
        }

        public async Task<(IEnumerable<User> Users, Pagination Pagination)> GetUsersAsync(PaginationQuery paginationQuery)
        {
            IEnumerable<User> users = 
                await _usersRepo.GetAsync(paginationQuery.PageSize, paginationQuery.PageNumber);
            int recordCount = paginationQuery.IncludeCount ? users.Count() : default;

            var metadata = new Pagination
            {
                PageNumber = paginationQuery.PageNumber,
                PageSize = paginationQuery.PageSize,
                TotalRecords = recordCount
            };

            return (users, metadata);
        }
    }
}
