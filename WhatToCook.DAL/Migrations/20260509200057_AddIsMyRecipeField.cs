using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatToCook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMyRecipeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMyRecipe",
                table: "Recipes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsMyRecipe" },
                values: new object[] { new DateTime(2026, 5, 9, 20, 0, 56, 616, DateTimeKind.Utc).AddTicks(8293), false });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsMyRecipe" },
                values: new object[] { new DateTime(2026, 5, 9, 20, 0, 56, 616, DateTimeKind.Utc).AddTicks(9992), false });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "IsMyRecipe" },
                values: new object[] { new DateTime(2026, 5, 9, 20, 0, 56, 616, DateTimeKind.Utc).AddTicks(9997), false });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "IsMyRecipe" },
                values: new object[] { new DateTime(2026, 5, 9, 20, 0, 56, 617, DateTimeKind.Utc), false });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "IsMyRecipe" },
                values: new object[] { new DateTime(2026, 5, 9, 20, 0, 56, 617, DateTimeKind.Utc).AddTicks(3), false });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "IsMyRecipe" },
                values: new object[] { new DateTime(2026, 5, 9, 20, 0, 56, 617, DateTimeKind.Utc).AddTicks(6), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMyRecipe",
                table: "Recipes");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 8, 23, 46, 50, 78, DateTimeKind.Local).AddTicks(5893));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 8, 23, 46, 50, 82, DateTimeKind.Local).AddTicks(7973));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 8, 23, 46, 50, 82, DateTimeKind.Local).AddTicks(7996));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 8, 23, 46, 50, 82, DateTimeKind.Local).AddTicks(8002));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 8, 23, 46, 50, 82, DateTimeKind.Local).AddTicks(8006));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 8, 23, 46, 50, 82, DateTimeKind.Local).AddTicks(8011));
        }
    }
}
