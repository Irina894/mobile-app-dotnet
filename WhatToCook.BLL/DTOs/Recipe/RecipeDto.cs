using WhatToCook.BLL.DTOs.Ingredient;

namespace WhatToCook.BLL.DTOs.Recipe;

public class RecipeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CookTimeMinutes { get; set; }
    public string MoodTag { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string? ImageUrl { get; set; }

    // Список інгредієнтів, які входять у цей рецепт
    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
}