using DataAccess.Entities;
using System.Linq.Expressions;

namespace DataAccess.Base
{
    public interface IBaseRepository<TEntity>
       where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize);
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<bool> Exists(int id);
    }
}
