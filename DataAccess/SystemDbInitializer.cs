using Core.Enums;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public static class SystemDbInitializer
    {
        private static readonly string UserAdminId = "51586e47-b125-4534-bba4-9bc6fd3dfbc8";
        private static readonly string UserGuestId = "61586e47-b125-4534-bba4-9bc6fd3dfbc8";


        public static void SeedData(this ModelBuilder modelBuilder) {
            SeedUsers(modelBuilder);
            SeedCategories(modelBuilder);
            SeedTodoList(modelBuilder);
        }

        private static void SeedUsers(ModelBuilder modelBuilder) {
            var ownerRoleId = "149b2f7f-8358-4f68-be8e-e17eddb9f025";
            var guestRoleId = "169b2f7f-8358-4f68-be8e-e17eddb9f027";

            modelBuilder.Entity<IdentityRole>().HasData(new List<IdentityRole> {
                new IdentityRole
                {
                    Id = ownerRoleId,
                    Name = "Owner",
                    NormalizedName = "OWNER",
                    ConcurrencyStamp = null
                },
                new IdentityRole
                {
                    Id = guestRoleId,
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    ConcurrencyStamp = null
                }
            });


            var adminUser = GenerateUser(UserAdminId, "Admin", "Admin@mail.com", "Owner", "Admin@12345");
            modelBuilder.Entity<User>().HasData(adminUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> {
                UserId = adminUser.Id,
                RoleId = ownerRoleId
            });

            var guestUser = GenerateUser(UserGuestId, "Guest", "Guest@mail.com", "Guest", "Guest@12345");
            modelBuilder.Entity<User>().HasData(guestUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> {
                UserId = guestUser.Id,
                RoleId = guestRoleId
            });
        }

        private static void SeedCategories(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Category>().HasData(new List<Category>
            {
                new()
                {
                    Id = 1,
                    Name = "Category A",
                    Created = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    CreatedBy = "System",
                    LastModifiedBy = string.Empty,
                    Color = "Red"
                },
                new()
                {
                    Id = 2,
                    Name = "Category B",
                    Created = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    CreatedBy = "System",
                    LastModifiedBy = string.Empty,
                    Color = "Blue"
                }
            });
        }

        private static void SeedTodoList(ModelBuilder modelBuilder) {
            modelBuilder.Entity<TodoItem>().HasData(new List<TodoItem>()
            {
                new ()
                {
                    Id = 1,
                    Title = "Todo #1",
                    Description = "Todo #1 Description",
                    CategoryId = 1,
                    Created = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    CreatedBy = "System",
                    IsCompleted = false,
                    LastModifiedBy = string.Empty,
                    Priority = PriorityLevel.Critical,
                    UserId = UserAdminId
                },
                new ()
                {
                    Id = 2,
                    Title = "Todo #2",
                    Description = "Todo #2 Description",
                    CategoryId = 1,
                    Created = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    CreatedBy = "System",
                    IsCompleted = false,
                    LastModifiedBy = string.Empty,
                    Priority = PriorityLevel.Low,
                    UserId = UserAdminId
                },
                new ()
                {
                    Id = 3,
                    Title = "Todo #3",
                    Description = "Todo #3 Description",
                    CategoryId = 2,
                    Created = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 06, 28, 0, 0, 0, 0),
                    CreatedBy = "System",
                    IsCompleted = false,
                    LastModifiedBy = string.Empty,
                    Priority = PriorityLevel.Medium,
                    UserId = UserAdminId
                },

            });
        }

        private static User GenerateUser(string id, string userName, string email, string role, string password) {
            var adminUser = new User {
                Id = id,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = false,
                FullName = userName,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false
            };
            adminUser.PasswordHash = new PasswordHasher<User>().HashPassword(adminUser, password);
            return adminUser;
        }
    }
}
