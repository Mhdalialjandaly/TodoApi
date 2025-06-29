using DataAccess.Entities;
using DataAccess.Services.Response;
using System.Security.Claims;

namespace DataAccess.Services
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user,IList<string> rols);
        Task<string> GenerateRefreshTokenAsync();
        Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);
        Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);
        Task RevokeTokenAsync(string userId);
    }
}
