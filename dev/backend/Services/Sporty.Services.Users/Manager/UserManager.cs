using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Infra.Data.Accessor.Mongo.Models;
using Sporty.Infra.Data.Accessor.Mongo.Repository;
using Sporty.Services.Users.DTO.Model;

namespace Sporty.Services.Users.Manager
{
    internal class UserManager : IUserManager
    {
        private const string c_collectionName = "users";
        private readonly ILogger<UserManager> _logger;
        private readonly MongoRepository<User, Guid> _usersRepo;

        public UserManager(IOptions<MongoConfiguration> options, ILogger<UserManager> logger)
        {
            MongoConfiguration mongoConfiguration = options.Value;
            
            _logger = logger;
            _usersRepo = new MongoRepository<User, Guid>(mongoConfiguration); 
            _usersRepo.Init(c_collectionName);

            _logger.LogInformation($"Initiated connection: {mongoConfiguration}");
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
