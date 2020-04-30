using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Services.Groups.Contracts;
using Sporty.Services.Groups.DTO.Model;

namespace Sporty.Services.Groups.Manager
{
    public interface IGroupManager : IRepositoryBase<Group>
    {
        Task<(IEnumerable<Group> Groups, Pagination Pagination)> GetGroupsAsync(PaginationQuery paginationQuery);
        Task<(int groupUpdated, int memberUpdated)> AddGroupMembersAsync(Guid id, IEnumerable<Guid> members);
        Task<(int groupUpdated, int memberUpdated)> DeleteGroupMembersAsync(Guid id, IEnumerable<Guid> members);
    }
}
