using Core.Enums;
using DataAccess.Base;
using DataAccess.Entities;

namespace DataAccess.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetUserWithTodosAsync(string userId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<int> GetUsersCountAsync();
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
