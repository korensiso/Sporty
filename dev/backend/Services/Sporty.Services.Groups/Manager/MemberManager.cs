using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sporty.Infra.Data.Accessor.Mongo.Models;
using Sporty.Infra.Data.Accessor.Mongo.Repository;
using Sporty.Services.Groups.DTO.Model;

namespace Sporty.Services.Groups.Manager
{
    public class MemberManager : IMemberManager
    {
        private const string c_collectionName = "members";
        private readonly ILogger<MemberManager> _logger;
        private readonly MongoRepository<Member, Guid> _membersRepo;

        public MemberManager(IOptions<MongoConfiguration> options, ILogger<MemberManager> logger)
        {
            MongoConfiguration mongoConfiguration = options.Value;

            _logger = logger;
            _membersRepo = new MongoRepository<Member, Guid>(mongoConfiguration);
            _membersRepo.Init(c_collectionName);

            _logger.LogInformation($"Initiated connection: {mongoConfiguration}");
        }

        public async Task<IEnumerable<Member>> GetAllAsync()
        {
            return await _membersRepo.GetAsync();

        }

        public async Task<Member> GetByIdAsync(Guid id)
        {
            Member member = await _membersRepo.FirstOrDefaultAsync(member => member.Identifier == id);
            var members = await _membersRepo.FindAsync(member => member.Identifier == id);
            members = await GetAllAsync();
            return member;
        }

        public async Task<Guid> CreateAsync(Member entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            await _membersRepo.AddAsync(entity);

            return entity.Identifier;
        }

        public async Task<bool> UpdateAsync(Member entity)
        {
            Member currentMember = await GetByIdAsync(entity.Identifier);
            if (currentMember == null) return false;

            long modified = await _membersRepo.UpdateAsync(member => member.Identifier == entity.Identifier,
                currentMember.Update(entity));

            return modified != 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            long deleted = await _membersRepo.DeleteAsync(member => member.Identifier == id);
            return deleted != 0;
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            return await GetByIdAsync(id) != null;
        }
    }
}
