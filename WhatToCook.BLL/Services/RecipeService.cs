using AutoMapper;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories; // Тут лежить IRepository від Іри

namespace WhatToCook.BLL.Services;

public class RecipeService : IRecipeService
{
    private readonly IRepository<Recipe> _recipeRepository;
    private readonly IMapper _mapper;

    // Впроваджуємо залежності (Dependency Injection)
    public RecipeService(IRepository<Recipe> recipeRepository, IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
    {
        // 1. Беремо дані з бази (Entities)
        var recipes = await _recipeRepository.GetAllAsync();

        // 2. Перетворюємо їх у DTO за допомогою AutoMapper
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) return null;

        return _mapper.Map<RecipeDto>(recipe);
    }

    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto)
    {
        var recipeEntity = _mapper.Map<Recipe>(dto);
        recipeEntity.CreatedAt = DateTime.UtcNow;

        await _recipeRepository.AddAsync(recipeEntity);

        return _mapper.Map<RecipeDto>(recipeEntity);
    }

    public async Task UpdateRecipeAsync(int id, UpdateRecipeDto dto)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException("Recipe not found.");
        }

        _mapper.Map(dto, recipe);
        await _recipeRepository.UpdateAsync(recipe);
    }

    public async Task DeleteRecipeAsync(int id)
    {
        await _recipeRepository.DeleteAsync(id);
    }
}