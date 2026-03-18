namespace WhatToCook.BLL.DTOs.Recipe;

public class CreateRecipeDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CookTimeMinutes { get; set; }
    public string MoodTag { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    // Ми можемо додавати рецепт відразу з текстом інструкції
    public string Instructions { get; set; } = string.Empty;
}
