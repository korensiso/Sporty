using System.Collections.Generic;
using System.Threading.Tasks;
using Sporty.Common.Network.Http.QueryUrl;
using Sporty.Services.Users.Contracts;
using Sporty.Services.Users.DTO.Model;

namespace Sporty.Services.Users.Manager
{
    public interface IUserManager : IRepositoryBase<User>
    {
        Task<(IEnumerable<User> Users, Pagination Pagination)> GetUsersAsync(PaginationQuery paginationQuery);
    }
}
