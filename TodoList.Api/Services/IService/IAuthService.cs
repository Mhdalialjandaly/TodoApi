using DataAccess.Services.Response;

namespace DataAccess.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> GetCurrentUserAsync(string userId);
    }
}
