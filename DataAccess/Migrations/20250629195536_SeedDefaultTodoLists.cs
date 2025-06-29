using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultTodoLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "Created", "CreatedBy", "DeletedBy", "DeletedDate", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, "Red", new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, "", "Category A" },
                    { 2, "Blue", new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, "", "Category B" }
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
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "51586e47-b125-4534-bba4-9bc6fd3dfbc8",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECL8N27v+k5F7HvlD8UoRFK7Ug2tfXYOOkCSySoYu6l61y4wFD8aVvUY/YBar/jGnw==");
        }
    }
}
