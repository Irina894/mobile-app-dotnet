namespace WhatToCook.BLL.DTOs.Recipe;

public class RecipeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Правильна назва — відповідає полю CookingTime в entity через маппінг
    public int CookTimeMinutes { get; set; }

    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int Servings { get; set; }
    public decimal Rating { get; set; }
    public string? ImageUrl { get; set; }

    // Поля для стилізації UI (потрібні колезі)
    public string AccentColor { get; set; } = "#1E40AF";
    public string AccentTextColor { get; set; } = "#1E3A8A";

    // Прапорець власного рецепту
    public bool IsMyRecipe { get; set; }

    public DateTime CreatedAt { get; set; }
}