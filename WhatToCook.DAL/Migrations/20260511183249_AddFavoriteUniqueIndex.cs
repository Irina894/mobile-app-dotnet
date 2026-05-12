using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatToCook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavoriteRecipes_UserId",
                table: "FavoriteRecipes");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 18, 32, 48, 530, DateTimeKind.Utc).AddTicks(9870));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 18, 32, 48, 531, DateTimeKind.Utc).AddTicks(1974));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 18, 32, 48, 531, DateTimeKind.Utc).AddTicks(1981));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 18, 32, 48, 531, DateTimeKind.Utc).AddTicks(1984));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 18, 32, 48, 531, DateTimeKind.Utc).AddTicks(1989));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 18, 32, 48, 531, DateTimeKind.Utc).AddTicks(1992));

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRecipes_UserId_RecipeId",
                table: "FavoriteRecipes",
                columns: new[] { "UserId", "RecipeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavoriteRecipes_UserId_RecipeId",
                table: "FavoriteRecipes");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 20, 0, 56, 616, DateTimeKind.Utc).AddTicks(8293));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 20, 0, 56, 616, DateTimeKind.Utc).AddTicks(9992));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 20, 0, 56, 616, DateTimeKind.Utc).AddTicks(9997));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 20, 0, 56, 617, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 20, 0, 56, 617, DateTimeKind.Utc).AddTicks(3));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 20, 0, 56, 617, DateTimeKind.Utc).AddTicks(6));

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRecipes_UserId",
                table: "FavoriteRecipes",
                column: "UserId");
        }
    }
}
