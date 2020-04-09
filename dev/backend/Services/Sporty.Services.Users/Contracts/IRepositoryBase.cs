using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sporty.Services.Users.Contracts
{
    public interface IRepositoryBase<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistAsync(Guid id);
    }
}
