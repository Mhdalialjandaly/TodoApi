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
        public string Role
        {
            get
            {
                var roleValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);               
                    return roleValue; 
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return null;

            return await _userManager.FindByIdAsync(UserId);
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return false;

            return await IsInRoleAsync(role);
        }

    }
}
