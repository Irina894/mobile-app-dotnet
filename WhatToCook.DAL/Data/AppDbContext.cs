using Microsoft.EntityFrameworkCore;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
            entity.HasIndex(u => u.Email).IsUnique(); // Унікальна пошта
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
    }
}