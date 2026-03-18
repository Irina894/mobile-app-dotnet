namespace WhatToCook.DAL.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CookTimeMinutes { get; set; }
    public string MoodTag { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    // Зв'язок: цей рецепт може бути в "улюблених" у багатьох користувачів
    public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();
}