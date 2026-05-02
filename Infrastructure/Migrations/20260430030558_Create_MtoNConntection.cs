using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create_MtoNConntection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 30, 3, 5, 58, 372, DateTimeKind.Utc).AddTicks(4148), null, null, false, "CreateUser", null },
                    { 2, new DateTime(2026, 4, 30, 3, 5, 58, 372, DateTimeKind.Utc).AddTicks(4530), null, null, false, "EditUser", null },
                    { 3, new DateTime(2026, 4, 30, 3, 5, 58, 372, DateTimeKind.Utc).AddTicks(4531), null, null, false, "DeleteUser", null },
                    { 4, new DateTime(2026, 4, 30, 3, 5, 58, 372, DateTimeKind.Utc).AddTicks(4542), null, null, false, "ViewUser", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 30, 3, 5, 58, 372, DateTimeKind.Utc).AddTicks(9608), null, false, "Admin", null },
                    { 2, new DateTime(2026, 4, 30, 3, 5, 58, 372, DateTimeKind.Utc).AddTicks(9901), null, false, "User", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
