using WhatToCook.BLL.DTOs.Recipe;

namespace WhatToCook.BLL.Interfaces;

public interface IRecipeService
{
    // Отримати всі рецепти для головного екрана
    Task<IEnumerable<RecipeDto>> GetAllRecipesAsync();

    // Отримати один рецепт з усіма інгредієнтами за ID
    Task<RecipeDto?> GetRecipeByIdAsync(int id);

    // Додати новий рецепт
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto);
    Task UpdateRecipeAsync(int id, UpdateRecipeDto dto);
        Task DeleteRecipeAsync(int id);
}