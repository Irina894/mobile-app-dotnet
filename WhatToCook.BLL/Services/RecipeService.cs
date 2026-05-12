using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Data;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class RecipeService : IRecipeService
{
    private readonly IRepository<Recipe> _recipeRepository;
    private readonly IRepository<RecipeIngredient> _recipeIngredientRepository;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFavoriteService _favoriteService;


    public RecipeService(
        IRepository<Recipe> recipeRepository,
        IRepository<RecipeIngredient> recipeIngredientRepository,
        AppDbContext context,
        IMapper mapper,
        IFavoriteService favoriteService)
    {
        _recipeRepository = recipeRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
        _context = context;
        _mapper = mapper;
        _favoriteService = favoriteService;
    }

    // ── GET ALL ───────────────────────────────────────────────────────────
    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync(int userId)
    {
        var recipes = await _recipeRepository.GetAllAsync();
        var dtos = _mapper.Map<List<RecipeDto>>(recipes);
        var favIds = await _favoriteService.GetFavoriteRecipeIdsAsync(userId);
        foreach (var dto in dtos)
            dto.IsFavorite = favIds.Contains(dto.Id);
        return dtos;
    }

    public async Task<IEnumerable<RecipeDto>> SearchRecipesAsync(
        List<int> ingredientIds, string? query, int userId)
    {
        var dtos = (await SearchRecipesAsync(ingredientIds, query)).ToList();
        var favIds = await _favoriteService.GetFavoriteRecipeIdsAsync(userId);
        foreach (var dto in dtos)
            dto.IsFavorite = favIds.Contains(dto.Id);
        return dtos;
    }

    // ── GET BY ID з Include ───────────────────────────────────────────────
    public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return null;
        return _mapper.Map<RecipeDto>(recipe);
    }

    // ── SEARCH ────────────────────────────────────────────────────────────
    // Широка фільтрація: рецепт показується якщо містить ХОЧА Б ОДИН
    // з вибраних інгредієнтів. Додатково фільтрує за назвою якщо є query.
    public async Task<IEnumerable<RecipeDto>> SearchRecipesAsync(
        List<int> ingredientIds,
        string? query)
    {
        // Починаємо з базового запиту з Include
        var recipesQuery = _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .AsNoTracking()
            .AsQueryable();

        // Фільтр за інгредієнтами (хоча б один збігається)
        if (ingredientIds.Any())
        {
            recipesQuery = recipesQuery.Where(r =>
                r.RecipeIngredients.Any(ri =>
                    ingredientIds.Contains(ri.IngredientId)));
        }

        // Фільтр за назвою (необов'язковий)
        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            recipesQuery = recipesQuery.Where(r =>
                r.Title.ToLower().Contains(q) ||
                r.Description.ToLower().Contains(q) ||
                r.Category.ToLower().Contains(q));
        }

        var recipes = await recipesQuery.ToListAsync();
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    // ── CREATE ────────────────────────────────────────────────────────────
    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto)
    {
        var recipeEntity = _mapper.Map<Recipe>(dto);
        recipeEntity.Rating = 0;
        recipeEntity.CreatedAt = DateTime.UtcNow;

        await _recipeRepository.AddAsync(recipeEntity);

        if (dto.IngredientIds.Any())
        {
            foreach (var ingredientId in dto.IngredientIds)
            {
                await _recipeIngredientRepository.AddAsync(new RecipeIngredient
                {
                    RecipeId = recipeEntity.Id,
                    IngredientId = ingredientId,
                    Quantity = 1,
                    Unit = "pcs"
                });
            }
        }

        var withIngredients = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == recipeEntity.Id);

        return _mapper.Map<RecipeDto>(withIngredients!);
    }

    // ── UPDATE ────────────────────────────────────────────────────────────
    public async Task UpdateRecipeAsync(int id, UpdateRecipeDto dto)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) throw new KeyNotFoundException($"Recipe {id} not found.");
        _mapper.Map(dto, recipe);
        await _recipeRepository.UpdateAsync(recipe);
    }

    // ── DELETE ────────────────────────────────────────────────────────────
    public async Task DeleteRecipeAsync(int id)
    {
        await _recipeRepository.DeleteAsync(id);
    }
}