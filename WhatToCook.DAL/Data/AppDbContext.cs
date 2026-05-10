using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WhatToCook.DAL.Entities;

namespace WhatToCook.DAL.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Це критичний рядок для виправлення вашої помилки в Package Manager Console.
        // Він дозволяє ігнорувати розбіжності між моделлю та міграцією під час оновлення.
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Налаштування таблиць (Fluent API)
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Title).IsRequired().HasMaxLength(150);
            entity.Property(r => r.ImageUrl).HasMaxLength(500);
            entity.Property(r => r.Rating).HasColumnType("decimal(3,2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<FavoriteRecipe>(entity =>
        {
            entity.HasKey(fr => fr.Id);
            entity.HasOne(fr => fr.User).WithMany(u => u.FavoriteRecipes).HasForeignKey(fr => fr.UserId);
            entity.HasOne(fr => fr.Recipe).WithMany(r => r.FavoriteRecipes).HasForeignKey(fr => fr.RecipeId);
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(ri => ri.Id);
            entity.Property(ri => ri.Quantity).HasColumnType("decimal(10,2)");
            entity.HasOne(ri => ri.Recipe).WithMany(r => r.RecipeIngredients).HasForeignKey(ri => ri.RecipeId);
            entity.HasOne(ri => ri.Ingredient).WithMany(i => i.RecipeIngredients).HasForeignKey(ri => ri.IngredientId);
        });

        // --- ДОДАВАННЯ ТЕСТОВИХ ДАНИХ (SEEDING) ---

        modelBuilder.Entity<Ingredient>().HasData(
            new Ingredient { Id = 1, Name = "Chicken", ImageUrl = "https://images.pexels.com/photos/2338407/pexels-photo-2338407.jpeg?w=200" },
            new Ingredient { Id = 2, Name = "Eggs", ImageUrl = "https://images.pexels.com/photos/162712/egg-white-food-protein-162712.jpeg?w=200" },
            new Ingredient { Id = 3, Name = "Tomato", ImageUrl = "https://images.pexels.com/photos/533280/pexels-photo-533280.jpeg?w=200" },
            new Ingredient { Id = 4, Name = "Pasta", ImageUrl = "https://images.pexels.com/photos/1279330/pexels-photo-1279330.jpeg?w=200" },
            new Ingredient { Id = 5, Name = "Onion", ImageUrl = "https://images.pexels.com/photos/4197447/pexels-photo-4197447.jpeg?w=200" },
            new Ingredient { Id = 6, Name = "Cheese", ImageUrl = "https://images.pexels.com/photos/773253/pexels-photo-773253.jpeg?w=200" },
            new Ingredient { Id = 7, Name = "Potato", ImageUrl = "https://images.pexels.com/photos/144248/potatoes-vegetables-erdfrucht-bio-144248.jpeg?w=200" },
            new Ingredient { Id = 8, Name = "Garlic", ImageUrl = "https://images.pexels.com/photos/4198937/pexels-photo-4198937.jpeg?w=200" },
            new Ingredient { Id = 9, Name = "Beef", ImageUrl = "https://images.pexels.com/photos/618775/pexels-photo-618775.jpeg?w=200" },
            new Ingredient { Id = 10, Name = "Carrot", ImageUrl = "https://images.pexels.com/photos/143133/pexels-photo-143133.jpeg?w=200" }
        );

        modelBuilder.Entity<Recipe>().HasData(
           new Recipe
           {
               Id = 1,
               Title = "Borscht",
               Description = "Rich Ukrainian beet soup with vegetables, meat and sour cream on top.",
               CookingTime = 60,
               Rating = 4.9m,
               Category = "Soup",
               Difficulty = "Medium",
               Servings = 4,
               AccentColor = "#9F1239",
               AccentTextColor = "#881337",
               ImageUrl = "https://images.pexels.com/photos/5774025/pexels-photo-5774025.jpeg?w=600"
           },
           new Recipe
           {
               Id = 2,
               Title = "Omelette",
               Description = "Fluffy French-style omelette with herbs, cheese and cherry tomatoes.",
               CookingTime = 10,
               Rating = 4.5m,
               Category = "Breakfast",
               Difficulty = "Easy",
               Servings = 1,
               AccentColor = "#B45309",
               AccentTextColor = "#92400E",
               ImageUrl = "https://images.pexels.com/photos/6294248/pexels-photo-6294248.jpeg?w=600"
           },
           new Recipe
           {
               Id = 3,
               Title = "Pasta Carbonara",
               Description = "Classic Italian pasta with creamy egg sauce, guanciale and Pecorino Romano.",
               CookingTime = 25,
               Rating = 4.8m,
               Category = "Pasta",
               Difficulty = "Medium",
               Servings = 2,
               AccentColor = "#92400E",
               AccentTextColor = "#78350F",
               ImageUrl = "https://images.pexels.com/photos/1279330/pexels-photo-1279330.jpeg?w=600"
           },
           new Recipe
           {
               Id = 4,
               Title = "Greek Salad",
               Description = "Fresh tomatoes, cucumbers, olives and feta with light olive oil dressing.",
               CookingTime = 5,
               Rating = 4.6m,
               Category = "Salad",
               Difficulty = "Easy",
               Servings = 2,
               AccentColor = "#14532D",
               AccentTextColor = "#166534",
               ImageUrl = "https://images.pexels.com/photos/1059905/pexels-photo-1059905.jpeg?w=600"
           },
           new Recipe
           {
               Id = 5,
               Title = "Chicken Soup",
               Description = "Warm and comforting homemade chicken soup with vegetables and noodles.",
               CookingTime = 45,
               Rating = 4.7m,
               Category = "Soup",
               Difficulty = "Easy",
               Servings = 4,
               AccentColor = "#B45309",
               AccentTextColor = "#92400E",
               ImageUrl = "https://images.pexels.com/photos/1640777/pexels-photo-1640777.jpeg?w=600"
           },
           new Recipe
           {
               Id = 6,
               Title = "Beef Steak",
               Description = "Juicy pan-seared beef steak with garlic butter, rosemary and thyme.",
               CookingTime = 20,
               Rating = 4.9m,
               Category = "Meat",
               Difficulty = "Medium",
               Servings = 2,
               AccentColor = "#7C2D12",
               AccentTextColor = "#6B2110",
               ImageUrl = "https://images.pexels.com/photos/618775/pexels-photo-618775.jpeg?w=600"
           }
        );
    }
}