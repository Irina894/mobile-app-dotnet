using AutoMapper;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class RecipeService : IRecipeService
{
    private readonly IRepository<Recipe> _recipeRepository;
    private readonly IMapper _mapper;

    public RecipeService(IRepository<Recipe> recipeRepository, IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllAsync();
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
        // Маппінг DTO → Entity
        var recipeEntity = _mapper.Map<Recipe>(dto);

        // Поля які задаємо тільки тут — не з форми
        recipeEntity.Rating = 0;
        recipeEntity.CreatedAt = DateTime.UtcNow;

        await _recipeRepository.AddAsync(recipeEntity);

        return _mapper.Map<RecipeDto>(recipeEntity);
    }

    public async Task UpdateRecipeAsync(int id, UpdateRecipeDto dto)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
            throw new KeyNotFoundException($"Recipe with id={id} not found.");

        _mapper.Map(dto, recipe);
        await _recipeRepository.UpdateAsync(recipe);
    }

    public async Task DeleteRecipeAsync(int id)
    {
        await _recipeRepository.DeleteAsync(id);
    }
}