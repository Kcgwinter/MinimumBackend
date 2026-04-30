using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken_BaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_RefreshTokens", table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "RefreshTokens",
                newName: "CreatedAt"
            );

            migrationBuilder
                .AddColumn<int>(
                    name: "Id",
                    table: "RefreshTokens",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: 0
                )
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RefreshTokens",
                type: "TEXT",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RefreshTokens",
                type: "INTEGER",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RefreshTokens",
                type: "TEXT",
                nullable: true
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_RefreshTokens", table: "RefreshTokens");

            migrationBuilder.DropColumn(name: "Id", table: "RefreshTokens");

            migrationBuilder.DropColumn(name: "DeletedAt", table: "RefreshTokens");

            migrationBuilder.DropColumn(name: "IsDeleted", table: "RefreshTokens");

            migrationBuilder.DropColumn(name: "UpdatedAt", table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RefreshTokens",
                newName: "Guid"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Guid"
            );
        }
    }
}
