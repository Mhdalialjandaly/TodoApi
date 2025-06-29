using Core.Enums;
using DataAccess.Base;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly UserManager<User> _userManager;
        public UserRepository(ApiDbContext context,UserManager<User> userManager) : base(context) {
            _userManager = userManager;
        }
        

        public async Task<User> GetUserWithTodosAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.TodoItems)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return users;
        }

        public async Task<int> GetUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
