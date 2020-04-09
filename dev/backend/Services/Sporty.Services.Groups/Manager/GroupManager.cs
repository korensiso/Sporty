using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sporty.Common.Dto.Group.Model;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;
using Sporty.Infra.Data.Accessor.Mongo.Models;
using Sporty.Infra.Data.Accessor.Mongo.Repository;

namespace Sporty.Services.Groups.Manager
{
    internal class GroupManager : IGroupManager
    {
        private readonly ILogger<GroupManager> _logger;
        private readonly IMongoConfiguration _mongoConfiguration;
        private readonly MongoRepository<Group, Guid> _groupsRepo;

        public GroupManager(IConfiguration config, ILogger<GroupManager> logger)
        {
            _logger = logger;

            string connectionString = string.Format(config.GetConnectionString("SQLDBConnectionString"), "localhost", "27017");
            _mongoConfiguration = new MongoConfiguration(connectionString, "sporty");
            _groupsRepo = new MongoRepository<Group, Guid>(_mongoConfiguration); 
            _groupsRepo.Init("Groups");
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            return await _groupsRepo.GetAsync();
        }

        public async Task<Group> GetByIdAsync(Guid id)
        {
            return await _groupsRepo.FirstOrDefaultAsync(group => group.Identifier == id);
        }

        public async Task<Guid> CreateAsync(Group entity)
        {
            entity.Identifier = Guid.NewGuid();
            entity.CreatedOn = DateTime.UtcNow;

            await _groupsRepo.AddAsync(entity);
            
            return entity.Identifier;
        }

        public async Task<bool> UpdateAsync(Group entity)
        {
            Group currentGroup = await GetByIdAsync(entity.Identifier);
            if (currentGroup == null) return false;

            long modified = await _groupsRepo.UpdateAsync(group => group.Identifier == entity.Identifier, 
                currentGroup.Update(entity));
            
            return modified != 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            long deleted = await _groupsRepo.DeleteAsync(group => group.Identifier == id);
            return deleted != 0;
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            return await GetByIdAsync(id) != null;
        }

        public async Task<(IEnumerable<Group> Groups, Pagination Pagination)> GetGroupsAsync(PaginationQuery paginationQuery)
        {
            IEnumerable<Group> groups = 
                await _groupsRepo.GetAsync(paginationQuery.PageSize, paginationQuery.PageNumber);
            int recordCount = paginationQuery.IncludeCount ? groups.Count() : default;

            var metadata = new Pagination
            {
                PageNumber = paginationQuery.PageNumber,
                PageSize = paginationQuery.PageSize,
                TotalRecords = recordCount
            };

            return (groups, metadata);
        }

        public async Task<bool> UpdateGroupUsersAsync(Guid id, IEnumerable<Guid> users)
        {
            Group currentGroup = await GetByIdAsync(id);
            if (currentGroup == null) return false;

            foreach (Guid user in users)
            {
                currentGroup.AddUser(user);
            }
            long modified = await _groupsRepo.UpdateAsync(group => group.Identifier == id, currentGroup);

            return modified != 0;
        }
    }
}
