using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public static class SystemDbInitializer
    {
        public static void SeedData(this ModelBuilder modelBuilder) {
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

        private static User GetAdminUser() {
            var hasher = new PasswordHasher<User>();
            var adminUser = new User {
                Id = "51586e47-b125-4534-bba4-9bc6fd3dfbc8",
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
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@12345");
            return adminUser;
        }
    }
}
