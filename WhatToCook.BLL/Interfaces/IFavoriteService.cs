using WhatToCook.BLL.DTOs.Recipe;
namespace WhatToCook.BLL.Interfaces;

public interface IFavoriteService
{
    Task<IEnumerable<FavoriteRecipeDto>> GetUserFavoritesAsync(int userId);
    Task<IEnumerable<RecipeDto>> GetUserFavoriteRecipesAsync(int userId);  // ← НОВЕ: повертає повноцінні RecipeDto
    Task<FavoriteRecipeDto> AddToFavoritesAsync(CreateFavoriteRecipeDto dto);
    Task RemoveFromFavoritesAsync(int userId, int recipeId);
    Task<bool> ToggleAsync(int userId, int recipeId);                     // ← НОВЕ
    Task<bool> IsFavoriteAsync(int userId, int recipeId);                 // ← НОВЕ
    Task<HashSet<int>> GetFavoriteRecipeIdsAsync(int userId);             // ← для проставляння IsFavorite у списках
}