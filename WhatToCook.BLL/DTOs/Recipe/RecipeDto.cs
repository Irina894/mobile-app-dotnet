namespace WhatToCook.BLL.DTOs.Recipe;

public class RecipeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CookTimeMinutes { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int Servings { get; set; }
    public decimal Rating { get; set; }
    public string? ImageUrl { get; set; }
    public string AccentColor { get; set; } = "#1E40AF";
    public string AccentTextColor { get; set; } = "#1E3A8A";
    public bool IsMyRecipe { get; set; }
    public bool IsFavorite { get; set; }   // ← НОВЕ ПОЛЕ
    public DateTime CreatedAt { get; set; }

    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
}