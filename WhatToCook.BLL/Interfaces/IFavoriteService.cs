using WhatToCook.BLL.DTOs.Recipe;
namespace WhatToCook.BLL.Interfaces;

public interface IFavoriteService
{
    Task<IEnumerable<FavoriteRecipeDto>> GetUserFavoritesAsync(int userId);
    Task<FavoriteRecipeDto> AddToFavoritesAsync(CreateFavoriteRecipeDto dto);
    Task RemoveFromFavoritesAsync(int userId, int recipeId);
}