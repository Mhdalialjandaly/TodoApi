using DataAccess;
using DataAccess.Entities;
using DataAccess.Services;
using DataAccess.Services.Response;
using Microsoft.AspNetCore.Identity;
using SendGrid.Helpers.Errors.Model;

namespace TodoList.Api.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly ApiDbContext _context;

        public AuthService(
            UserManager<User> userManager,
            ApiDbContext context,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if email is already used
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new BadRequestException("Email already in use");

            // Check if role exists
            var existingRole = await _roleManager.FindByNameAsync(request.Role);
            if (existingRole == null)
                throw new BadRequestException("Role does not exist");

            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName ?? request.Email, // Default to email if username is null
                FullName = request.FirstName + " " + request.LastName,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException($"User creation failed: {errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, existingRole.Name);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Adding to role failed: {errors}");
                }

                var roles = new List<string> { existingRole.Name };

                return new AuthResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Token = await _tokenService.GenerateTokenAsync(user, roles)
                };
            }
            catch (Exception ex)
            {
                // Add actual logging in production
                throw new Exception($"An error occurred during registration: {ex.Message}", ex);
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new BadRequestException("Invalid credentials");

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateTokenAsync(user, userRoles);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: false);

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryDays"))
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName,
                Role = userRoles,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<double>("JwtSettings:TokenExpiryMinutes")),
                RefreshToken = refreshToken
            };
        }

        public async Task<UserResponse> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var userRoles = await _userManager.GetRolesAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName,
                LastName = user.FullName,
                Role = userRoles
            };
        }
    }
}
