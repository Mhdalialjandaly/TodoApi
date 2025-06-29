using Core.Enums;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public static class SystemDbInitializer
    {
        private static readonly string UserAdminId = "51586e47-b125-4534-bba4-9bc6fd3dfbc8";


        public static void SeedData(this ModelBuilder modelBuilder) {
            SeedUsers(modelBuilder);
            SeedCategories(modelBuilder);
            SeedTodoList(modelBuilder);
        }

        private static void SeedUsers(ModelBuilder modelBuilder) {
            var ownerRoleId = "149b2f7f-8358-4f68-be8e-e17eddb9f025";

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
                    Id = "169b2f7f-8358-4f68-be8e-e17eddb9f027",
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    ConcurrencyStamp = null
                }
            });


            var adminUser = GetAdminUser();
            modelBuilder.Entity<User>().HasData(adminUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> {
                UserId = adminUser.Id,
                RoleId = ownerRoleId
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

        private static User GetAdminUser() {
            var adminUser = new User {
                Id = UserAdminId,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "Admin@mail.com",
                NormalizedEmail = "ADMIN@MAIL.COM",
                EmailConfirmed = false,
                FullName = "Administrator",
                SecurityStamp = "R5KYJ6YWCF5JOO3OKYALJ7BICHJU5LAB",
                ConcurrencyStamp = "a9cbee59-a4bf-485b-a215-fc7835066d93",
                LockoutEnabled = false
            };
            adminUser.PasswordHash = new PasswordHasher<User>().HashPassword(adminUser, "Admin@12345");
            return adminUser;
        }
    }
}
