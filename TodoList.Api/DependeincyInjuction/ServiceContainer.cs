using Microsoft.AspNetCore.Identity;
using DataAccess.Entities;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Base;
using DataAccess.IRepositories;
using DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Jose;
using TodoList.Api.Services.Service;
using AutoMapper;
using TodoList.Api.RequestModel;
using DataAccess.ValidatorModel;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace TodoList.Api.DependeincyInjuction
{
    public static class ServiceContainer
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration) {
            var configuration1 = new MapperConfiguration(e =>
               e.AddProfiles(new List<Profile> {
                    new SystemMapping(),
                    new RequestMappingProfile()
               }));
            services.AddAutoMapper(typeof(Profile).Assembly);
            var mapper = configuration1.CreateMapper();
            services.AddSingleton(mapper);

            // DbContext
            services.AddDbContext<ApiDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));

            // Identity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApiDbContext>()
                .AddDefaultTokenProviders();

            // Repositories
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<ITodoItemRepository, TodoItemRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddHttpContextAccessor();
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            var jwtSettings = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
            };

            services.AddSingleton(tokenValidationParameters);

            //  TokenService
            services.AddScoped<ITokenService, TokenService>();

            //Validator Service
            services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            services.AddControllers();
            return services;
        }
    }
}
