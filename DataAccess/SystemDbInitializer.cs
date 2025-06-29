using Core.Enums;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace DataAccess
{
     public static class SystemDbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // إنشاء الأدوار إذا لم تكن موجودة
            string[] roleNames = { "Admin", "Owner","Guest"};

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }

            // إنشاء مستخدم Admin إذا لم يكن موجوداً
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var admin = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FullName = "مدير النظام"
                };

                var createAdmin = await userManager.CreateAsync(admin, "Admin@12345");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<User>()
            //    .HasData(new User
            //    {
            //        Id = "1",
            //        UserName = "Ali",
            //        PasswordHash = "1",
            //        PhoneNumber = "0941390732",
            //        PhoneNumberConfirmed = true,
            //        Email = "mahammaali89@gmail.com",
            //        EmailConfirmed = true,
            //        FullName ="mahammad ali",
            //        Role = UserRole.Owner,
            //    });
            //modelBuilder.Entity<User>()
            //    .HasData(new User
            //    {
            //        Id = "2",
            //        UserName = "Mahammad",
            //        PasswordHash = "2",
            //        PhoneNumber = "0941390732",
            //        PhoneNumberConfirmed = true,
            //        Email = "mahammaali89@gmail.com",
            //        EmailConfirmed = true,
            //        FullName = "mahammad ali",
            //        Role = UserRole.Guest,
            //    });

        }
    }
}
