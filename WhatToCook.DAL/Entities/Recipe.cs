using System.Collections.Generic;

namespace WhatToCook.DAL.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Назва в БД — CookingTime (не чіпаємо, маппінг вирішить різницю з DTO)
    public int CookingTime { get; set; }

    public int Servings { get; set; }
    public string Difficulty { get; set; } = "Medium";
    public string Category { get; set; } = "Dinner";
    public decimal Rating { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    // Поля для стилізації UI колеги
    public string AccentColor { get; set; } = "#1E40AF";
    public string AccentTextColor { get; set; } = "#1E3A8A";

    // Чи є це власним рецептом користувача (додається через AddRecipePage)
    public bool IsMyRecipe { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Зв'язки
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();
}