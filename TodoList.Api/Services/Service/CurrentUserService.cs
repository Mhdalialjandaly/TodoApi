using Core.Enums;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DataAccess.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        public UserRole Role
        {
            get
            {
                var roleValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(roleValue))
                    return UserRole.Guest; 

                try
                {
                    return Enum.Parse<UserRole>(roleValue);
                }
                catch
                {
                    return UserRole.Guest; 
                }
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return null;

            return await _userManager.FindByIdAsync(UserId);
        }

        public async Task<bool> IsInRoleAsync(UserRole role)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return false;

            return user.Role == role;
        }

        public async Task<bool> IsOwnerAsync()
        {
            return await IsInRoleAsync(UserRole.Owner);
        }

        public async Task<bool> IsGuestAsync()
        {
            return await IsInRoleAsync(UserRole.Guest);
        }
    }
}
