using DataAccess.Entities;
using DataAccess.Services.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DataAccess.Services
{
    // Infrastructure/Services/TokenService.cs
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ApiDbContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenService(
            IConfiguration configuration,
            UserManager<User> userManager,
            ApiDbContext context,
            TokenValidationParameters tokenValidationParameters)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<double>("JwtSettings:TokenExpiryMinutes")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = _tokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false;

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = await GetPrincipalFromExpiredTokenAsync(token);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new SecurityTokenException("Invalid user");

            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && x.UserId == userId);

            if (storedRefreshToken == null ||
                storedRefreshToken.ExpiryDate < DateTime.UtcNow ||
                storedRefreshToken.IsRevoked)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            // Generate new token
            var newToken = await GenerateTokenAsync(user);
            var newRefreshToken = await GenerateRefreshTokenAsync();

            // Revoke old refresh token
            storedRefreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(storedRefreshToken);

            // Save new refresh token
            var newStoredRefreshToken = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = userId,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryDays"))
            };

            await _context.RefreshTokens.AddAsync(newStoredRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName,
                Role = user.Role,
                Token = newToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<double>("JwtSettings:TokenExpiryMinutes")),
                RefreshToken = newRefreshToken
            };
        }

        public async Task RevokeTokenAsync(string userId)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
            }

            _context.RefreshTokens.UpdateRange(refreshTokens);
            await _context.SaveChangesAsync();
        }
    }
}
