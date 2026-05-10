using AutoMapper;
using WhatToCook.BLL.DTOs.Ingredient;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class IngredientService : IIngredientService
{
    private readonly IRepository<Ingredient> _ingredientRepository;
    private readonly IMapper _mapper;

    public IngredientService(IRepository<Ingredient> ingredientRepository, IMapper mapper)
    {
        _ingredientRepository = ingredientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync()
    {
        var ingredients = await _ingredientRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
    }

    public async Task<IngredientDto?> GetIngredientByIdAsync(int id)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id);

        if (ingredient == null)
        {
            return null;
        }

        return _mapper.Map<IngredientDto>(ingredient);
    }

    public async Task<IngredientDto> CreateIngredientAsync(CreateIngredientDto dto)
    {
        var ingredientEntity = _mapper.Map<Ingredient>(dto);

        await _ingredientRepository.AddAsync(ingredientEntity);

        return _mapper.Map<IngredientDto>(ingredientEntity);
    }

    public async Task UpdateIngredientAsync(int id, UpdateIngredientDto dto)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id);

        if (ingredient == null)
        {
            throw new KeyNotFoundException("Ingredient not found.");
        }

        _mapper.Map(dto, ingredient);
        await _ingredientRepository.UpdateAsync(ingredient);
    }

    public async Task DeleteIngredientAsync(int id)
    {
        await _ingredientRepository.DeleteAsync(id);
    }
}