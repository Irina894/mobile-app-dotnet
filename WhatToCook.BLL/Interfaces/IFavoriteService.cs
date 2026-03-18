using WhatToCook.BLL.DTOs.Recipe;
namespace WhatToCook.BLL.Interfaces;

public interface IFavoriteService
{
    Task AddToFavoriteAsync(int userId, int recipeId);
    Task<IEnumerable<FavoriteRecipeDto>> GetUserFavoritesAsync(int userId);
}