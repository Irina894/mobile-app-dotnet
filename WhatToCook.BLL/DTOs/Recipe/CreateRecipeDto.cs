namespace WhatToCook.BLL.DTOs.Recipe;

public class CreateRecipeDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // MAUI надсилає CookTimeMinutes — маппінг переведе в CookingTime
    public int CookTimeMinutes { get; set; }

    public string Category { get; set; } = "Dinner";
    public string Difficulty { get; set; } = "Medium";
    public int Servings { get; set; } = 2;
    public string? ImageUrl { get; set; }

    public string AccentColor { get; set; } = "#1E40AF";
    public string AccentTextColor { get; set; } = "#1E3A8A";

    public bool IsMyRecipe { get; set; }
}