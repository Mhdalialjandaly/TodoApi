using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using DataAccess.Entities;
using DataAccess.Services.Response;
using SendGrid.Helpers.Errors.Model;

namespace DataAccess.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ApiDbContext _context;
        public AuthService(
            ApiDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ITokenService tokenService) {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request) {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new BadRequestException("Email already in use");

            var user = new User {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FirstName,
                Role = request.Role
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.First().Description);

            return new AuthResponse {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.GenerateTokenAsync(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request) {
            var role = new IdentityRole("Admin");
            var result = await _roleManager.CreateAsync(role);

            var usr = await _userManager.CreateAsync(new User() {
                FullName = "Administrator",
                UserName = "Admin",
                Email = "Admin@mail.com",
            });
            if (usr.Succeeded) {
                var cusr = await _userManager.FindByNameAsync("Admin");
                await _userManager.AddPasswordAsync(cusr, "Admin@123");
            }


            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new BadRequestException("Invalid credentials");

            var token = await _tokenService.GenerateTokenAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken {
                Token = refreshToken,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryDays"))
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponse {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName,
                Role = user.Role,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<double>("JwtSettings:TokenExpiryMinutes")),
                RefreshToken = refreshToken
            };
        }

        public async Task<UserResponse> GetCurrentUserAsync(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            return new UserResponse {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName,
                Role = user.Role
            };
        }
    }
}
