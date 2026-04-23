using WhatToCook.MAUI.Models;

namespace WhatToCook.MAUI.Services
{
    public interface IRecipeApiService
    {
        Task<IEnumerable<RecipeModel>> GetAllAsync();
        Task<RecipeModel?> GetByIdAsync(int id);
        Task<RecipeModel?> CreateAsync(RecipeModel recipe);
        Task<bool> UpdateAsync(RecipeModel recipe);
        Task<bool> DeleteAsync(int id);
    }
}