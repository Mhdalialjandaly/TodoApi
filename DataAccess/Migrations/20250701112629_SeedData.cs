using System;
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
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "51586e47-b125-4534-bba4-9bc6fd3dfbc8", 0, "b21eda53-5ff1-46b0-89d9-6cb47141b269", "Admin@mail.com", false, "Admin", false, null, "ADMIN@MAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEEexTq8fPDtetT25vQ1kkwdLe8GcokWl2TZDe4TAOvcsmAxefXkFp3GI0klfheTgYw==", null, false, "99dfad27-65dc-40e0-8b05-bde7826025f0", false, "Admin" },
                    { "61586e47-b125-4534-bba4-9bc6fd3dfbc8", 0, "c82c62ca-9786-4894-9248-3535a660e191", "Guest@mail.com", false, "Guest", false, null, "GUEST@MAIL.COM", "GUEST", "AQAAAAIAAYagAAAAEHS63mZhi9JVOjnK5aA8CBPCKWDhb9dK6r9GQNX4v4b8OxaqF0hn9OhvIpJmPiBXwA==", null, false, "465d47d5-4ce3-4a09-985e-b07d263b7aba", false, "Guest" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "Created", "CreatedBy", "DeletedBy", "DeletedDate", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, "Red", new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, "", "Category A" },
                    { 2, "Blue", new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, "", "Category B" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "149b2f7f-8358-4f68-be8e-e17eddb9f025", "51586e47-b125-4534-bba4-9bc6fd3dfbc8" },
                    { "169b2f7f-8358-4f68-be8e-e17eddb9f027", "61586e47-b125-4534-bba4-9bc6fd3dfbc8" }
                });

            migrationBuilder.InsertData(
                table: "TodoItems",
                columns: new[] { "Id", "CategoryId", "Created", "CreatedBy", "DeletedBy", "DeletedDate", "Description", "IsCompleted", "LastModified", "LastModifiedBy", "Priority", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, "Todo #1 Description", false, null, "", 3, "Todo #1", new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "51586e47-b125-4534-bba4-9bc6fd3dfbc8" },
                    { 2, 1, new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, "Todo #2 Description", false, null, "", 0, "Todo #2", new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "51586e47-b125-4534-bba4-9bc6fd3dfbc8" },
                    { 3, 2, new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, "Todo #3 Description", false, null, "", 1, "Todo #3", new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "51586e47-b125-4534-bba4-9bc6fd3dfbc8" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "149b2f7f-8358-4f68-be8e-e17eddb9f025", "51586e47-b125-4534-bba4-9bc6fd3dfbc8" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "169b2f7f-8358-4f68-be8e-e17eddb9f027", "61586e47-b125-4534-bba4-9bc6fd3dfbc8" });

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "149b2f7f-8358-4f68-be8e-e17eddb9f025");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "169b2f7f-8358-4f68-be8e-e17eddb9f027");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "51586e47-b125-4534-bba4-9bc6fd3dfbc8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "61586e47-b125-4534-bba4-9bc6fd3dfbc8");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
