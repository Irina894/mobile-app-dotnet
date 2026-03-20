using WhatToCook.BLL.DTOs.Ingredient;
namespace WhatToCook.BLL.Interfaces;

public interface IIngredientService
{
    Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync();
    Task<IngredientDto?> GetIngredientByIdAsync(int id);
    Task<IngredientDto> CreateIngredientAsync(CreateIngredientDto dto);
    Task UpdateIngredientAsync(int id, UpdateIngredientDto dto);
    Task DeleteIngredientAsync(int id);
}