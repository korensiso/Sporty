using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Infra.Data.Accessor.Mongo.Models;
using Sporty.Infra.Data.Accessor.Mongo.Repository;
using Sporty.Services.Groups.DTO.Model;

namespace Sporty.Services.Groups.Manager
{
    internal class GroupManager : IGroupManager
    {
        private const string c_collectionName = "groups";
        private readonly ILogger<GroupManager> _logger;
        private readonly MongoRepository<Group, Guid> _groupsRepo;

        public GroupManager(IOptions<MongoConfiguration> options, ILogger<GroupManager> logger)
            {
            MongoConfiguration mongoConfiguration = options.Value;

            _logger = logger;
            _groupsRepo = new MongoRepository<Group, Guid>(mongoConfiguration);
            _groupsRepo.Init(c_collectionName);

            _logger.LogInformation($"Initiated connection: {mongoConfiguration}");
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
            Group groupToUpdate = await GetByIdAsync(entity.Identifier);
            if (groupToUpdate == null) return false;

            long modified = await _groupsRepo.UpdateAsync(group => group.Identifier == entity.Identifier, entity);
            
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

        public async Task<(int groupUpdated, int memberUpdated)> AddGroupMembersAsync(Guid id, IEnumerable<Guid> members)
        {
            Group groupToUpdate = await GetByIdAsync(id);
            if (groupToUpdate == null) return (0,0);

            int updateCounter = 0;
            foreach (Guid member in members)
            {
                if (!groupToUpdate.ContainsMember(member))
                {
                    groupToUpdate.AddMember(member);
                }
                updateCounter++;
            }
            long modified = await _groupsRepo.UpdateAsync(group => group.Identifier == id, groupToUpdate);

            return ((int) modified, updateCounter);
        }

        public async Task<(int groupUpdated, int memberUpdated)> DeleteGroupMembersAsync(Guid id, IEnumerable<Guid> members)
        {
            Group groupToUpdate = await GetByIdAsync(id);
            if (groupToUpdate == null) return (0, 0);

            int updateCounter = 0;
            foreach (Guid member in members)
            {
                groupToUpdate.RemoveMember(member);
                updateCounter++;
            }
            long modified = await _groupsRepo.UpdateAsync(group => group.Identifier == id, groupToUpdate);

            return ((int)modified, updateCounter);
        }
    }
}
