using WhatToCook.MAUI.Models; // Тільки наша єдина модель

namespace WhatToCook.MAUI.Services.Interfaces;

public interface IRecipeApiService
{
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<IEnumerable<IngredientItem>> GetAllIngredientsAsync();
    Task<Recipe?> GetByIdAsync(int id);      
    Task<Recipe?> CreateAsync(Recipe recipe);
    Task<bool> UpdateAsync(Recipe recipe);   
    Task<bool> DeleteAsync(int id);
}