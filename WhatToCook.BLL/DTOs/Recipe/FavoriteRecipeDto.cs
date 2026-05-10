namespace WhatToCook.BLL.DTOs.Recipe;

public class FavoriteRecipeDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RecipeId { get; set; }

    public string RecipeTitle { get; set; } = string.Empty;
    public string? RecipeImageUrl { get; set; } // Можна навіть картинку додати для краси
}