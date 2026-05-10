using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WhatToCook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Calories = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAllergen = table.Column<bool>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CookingTime = table.Column<int>(type: "INTEGER", nullable: false),
                    Servings = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AccentColor = table.Column<string>(type: "TEXT", nullable: false),
                    AccentTextColor = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IngredientId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: false),
                    IsOptional = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteRecipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteRecipes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Calories", "Category", "ImageUrl", "IsAllergen", "Name" },
                values: new object[,]
                {
                    { 1, 0, "", "https://images.pexels.com/photos/2338407/pexels-photo-2338407.jpeg?w=200", false, "Chicken" },
                    { 2, 0, "", "https://images.pexels.com/photos/162712/egg-white-food-protein-162712.jpeg?w=200", false, "Eggs" },
                    { 3, 0, "", "https://images.pexels.com/photos/533280/pexels-photo-533280.jpeg?w=200", false, "Tomato" },
                    { 4, 0, "", "https://images.pexels.com/photos/1279330/pexels-photo-1279330.jpeg?w=200", false, "Pasta" },
                    { 5, 0, "", "https://images.pexels.com/photos/4197447/pexels-photo-4197447.jpeg?w=200", false, "Onion" },
                    { 6, 0, "", "https://images.pexels.com/photos/773253/pexels-photo-773253.jpeg?w=200", false, "Cheese" },
                    { 7, 0, "", "https://images.pexels.com/photos/144248/potatoes-vegetables-erdfrucht-bio-144248.jpeg?w=200", false, "Potato" },
                    { 8, 0, "", "https://images.pexels.com/photos/4198937/pexels-photo-4198937.jpeg?w=200", false, "Garlic" },
                    { 9, 0, "", "https://images.pexels.com/photos/618775/pexels-photo-618775.jpeg?w=200", false, "Beef" },
                    { 10, 0, "", "https://images.pexels.com/photos/143133/pexels-photo-143133.jpeg?w=200", false, "Carrot" }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "AccentColor", "AccentTextColor", "Category", "CookingTime", "CreatedAt", "Description", "Difficulty", "ImageUrl", "Rating", "Servings", "Title" },
                values: new object[,]
                {
                    { 1, "#9F1239", "#881337", "Soup", 60, new DateTime(2026, 5, 8, 23, 43, 10, 681, DateTimeKind.Local).AddTicks(7974), "Rich Ukrainian beet soup with vegetables, meat and sour cream on top.", "Medium", "https://images.pexels.com/photos/5774025/pexels-photo-5774025.jpeg?w=600", 4.9m, 4, "Borscht" },
                    { 2, "#B45309", "#92400E", "Breakfast", 10, new DateTime(2026, 5, 8, 23, 43, 10, 685, DateTimeKind.Local).AddTicks(2287), "Fluffy French-style omelette with herbs, cheese and cherry tomatoes.", "Easy", "https://images.pexels.com/photos/6294248/pexels-photo-6294248.jpeg?w=600", 4.5m, 1, "Omelette" },
                    { 3, "#92400E", "#78350F", "Pasta", 25, new DateTime(2026, 5, 8, 23, 43, 10, 685, DateTimeKind.Local).AddTicks(2313), "Classic Italian pasta with creamy egg sauce, guanciale and Pecorino Romano.", "Medium", "https://images.pexels.com/photos/1279330/pexels-photo-1279330.jpeg?w=600", 4.8m, 2, "Pasta Carbonara" },
                    { 4, "#14532D", "#166534", "Salad", 5, new DateTime(2026, 5, 8, 23, 43, 10, 685, DateTimeKind.Local).AddTicks(2332), "Fresh tomatoes, cucumbers, olives and feta with light olive oil dressing.", "Easy", "https://images.pexels.com/photos/1059905/pexels-photo-1059905.jpeg?w=600", 4.6m, 2, "Greek Salad" },
                    { 5, "#B45309", "#92400E", "Soup", 45, new DateTime(2026, 5, 8, 23, 43, 10, 685, DateTimeKind.Local).AddTicks(2336), "Warm and comforting homemade chicken soup with vegetables and noodles.", "Easy", "https://images.pexels.com/photos/1640777/pexels-photo-1640777.jpeg?w=600", 4.7m, 4, "Chicken Soup" },
                    { 6, "#7C2D12", "#6B2110", "Meat", 20, new DateTime(2026, 5, 8, 23, 43, 10, 685, DateTimeKind.Local).AddTicks(2339), "Juicy pan-seared beef steak with garlic butter, rosemary and thyme.", "Medium", "https://images.pexels.com/photos/618775/pexels-photo-618775.jpeg?w=600", 4.9m, 2, "Beef Steak" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRecipes_RecipeId",
                table: "FavoriteRecipes",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRecipes_UserId",
                table: "FavoriteRecipes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_RecipeId",
                table: "RecipeIngredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteRecipes");

            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
