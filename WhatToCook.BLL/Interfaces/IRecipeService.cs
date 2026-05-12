using WhatToCook.BLL.DTOs.Recipe;
namespace WhatToCook.BLL.Interfaces;

public interface IRecipeService
{
    Task<IEnumerable<RecipeDto>> GetAllRecipesAsync();
    Task<IEnumerable<RecipeDto>> GetAllRecipesAsync(int userId);                  // ← НОВЕ
    Task<RecipeDto?> GetRecipeByIdAsync(int id);
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto);
    Task UpdateRecipeAsync(int id, UpdateRecipeDto dto);
    Task DeleteRecipeAsync(int id);
    Task<IEnumerable<RecipeDto>> SearchRecipesAsync(List<int> ingredientIds, string? query);
    Task<IEnumerable<RecipeDto>> SearchRecipesAsync(List<int> ingredientIds, string? query, int userId); // ← НОВЕ
}