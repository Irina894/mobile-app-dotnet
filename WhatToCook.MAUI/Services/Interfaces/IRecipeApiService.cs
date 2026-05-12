using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.ViewModels;

namespace WhatToCook.MAUI.Services.Interfaces;

public interface IRecipeApiService
{
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<IEnumerable<IngredientItem>> GetAllIngredientsAsync();
    Task<Recipe?> GetByIdAsync(int id);
    Task<RecipeDetailModel?> GetByIdWithIngredientsAsync(int id);
    Task<Recipe?> CreateAsync(Recipe recipe);
    Task<Recipe?> CreateWithIngredientsAsync(object payload);
    Task<bool> UpdateAsync(Recipe recipe);
    Task<bool> DeleteAsync(int id);

    // Пошук рецептів за інгредієнтами та/або назвою
    Task<IEnumerable<Recipe>> SearchAsync(List<int> ingredientIds, string? query = null);
}