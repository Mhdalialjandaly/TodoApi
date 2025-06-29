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
            ITokenService tokenService) {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request) {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new BadRequestException("Email already in use");

            var existingRole = await _roleManager.FindByNameAsync(request.Role);
            if (existingRole == null)
                throw new BadRequestException("Role does not exist");

            var user = new User {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FirstName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.First().Description);

             await _userManager.AddToRoleAsync(user, existingRole.Name);
            var x = new List<string> { existingRole.Name};
            return new AuthResponse {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.GenerateTokenAsync(user, x)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request) {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new BadRequestException("Invalid credentials");
            var userRole = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateTokenAsync(user,userRole);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();
            await _signInManager.PasswordSignInAsync(user, request.Password,request.RememberMe, lockoutOnFailure: false);
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
                Role = userRole,
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
            var userRole = await _userManager.GetRolesAsync(user);
            return new UserResponse {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName,
                Role = userRole,
            };
        }
    }
}
