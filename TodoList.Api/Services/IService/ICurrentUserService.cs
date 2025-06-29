using Core.Enums;
using DataAccess.Entities;

namespace DataAccess.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Email { get; }
        string Role { get; }
        Task<User> GetCurrentUserAsync();
        Task<bool> IsInRoleAsync(string role);      
    }
}
