using WhatToCook.MAUI.Models;
namespace WhatToCook.MAUI.Services.Interfaces;

public interface IFavoriteApiService
{
    Task<bool> ToggleAsync(int userId, int recipeId);
    Task<IEnumerable<Recipe>> GetUserFavoriteRecipesAsync(int userId);
}