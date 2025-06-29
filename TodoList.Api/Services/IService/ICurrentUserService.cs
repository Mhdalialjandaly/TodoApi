using Core.Enums;
using DataAccess.Entities;

namespace DataAccess.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Email { get; }
        UserRole Role { get; }
        Task<User> GetCurrentUserAsync();
        Task<bool> IsInRoleAsync(UserRole role);
        Task<bool> IsOwnerAsync();
        Task<bool> IsGuestAsync();
    }
}
