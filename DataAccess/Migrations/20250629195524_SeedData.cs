using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "149b2f7f-8358-4f68-be8e-e17eddb9f025", null, "Owner", "OWNER" },
                    { "169b2f7f-8358-4f68-be8e-e17eddb9f027", null, "Guest", "GUEST" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Role", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "51586e47-b125-4534-bba4-9bc6fd3dfbc8", 0, "a9cbee59-a4bf-485b-a215-fc7835066d93", "Admin@mail.com", false, "Administrator", false, null, "ADMIN@MAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAECL8N27v+k5F7HvlD8UoRFK7Ug2tfXYOOkCSySoYu6l61y4wFD8aVvUY/YBar/jGnw==", null, false, 0, "R5KYJ6YWCF5JOO3OKYALJ7BICHJU5LAB", false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "149b2f7f-8358-4f68-be8e-e17eddb9f025", "51586e47-b125-4534-bba4-9bc6fd3dfbc8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "169b2f7f-8358-4f68-be8e-e17eddb9f027");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "149b2f7f-8358-4f68-be8e-e17eddb9f025", "51586e47-b125-4534-bba4-9bc6fd3dfbc8" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "149b2f7f-8358-4f68-be8e-e17eddb9f025");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "51586e47-b125-4534-bba4-9bc6fd3dfbc8");
        }
    }
}
