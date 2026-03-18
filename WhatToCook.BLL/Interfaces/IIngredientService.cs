using WhatToCook.BLL.DTOs.Ingredient;
namespace WhatToCook.BLL.Interfaces;

public interface IIngredientService
{
    Task<IEnumerable<IngredientDto>> GetAllAsync();
    Task<IngredientDto> CreateAsync(IngredientDto dto);
}