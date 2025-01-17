﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sporty.Common.Dto.Group.Model;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Services.Groups.Contracts;

namespace Sporty.Services.Groups.Manager
{
    public interface IGroupManager : IRepositoryBase<Group>
    {
        Task<(IEnumerable<Group> Groups, Pagination Pagination)> GetGroupsAsync(PaginationQuery paginationQuery);
        Task<bool> UpdateGroupUsersAsync(Guid id, IEnumerable<Guid> users);
    }
}
