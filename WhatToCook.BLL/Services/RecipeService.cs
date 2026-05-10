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
    private readonly AppDbContext _context; // для Include запитів
    private readonly IMapper _mapper;

    public RecipeService(
        IRepository<Recipe> recipeRepository,
        IRepository<RecipeIngredient> recipeIngredientRepository,
        AppDbContext context,
        IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
        _context = context;
        _mapper = mapper;
    }

    // ── GET ALL — без Include (для списку швидко) ─────────────────────────
    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    // ── GET BY ID — з Include інгредієнтів ───────────────────────────────
    public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
    {
        // Generic Repository не вміє Include — використовуємо DbContext напряму
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return null;
        return _mapper.Map<RecipeDto>(recipe);
    }

    // ── CREATE — зберігаємо рецепт + зв'язки з інгредієнтами ────────────
    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto)
    {
        // 1. Маппінг і збереження рецепту через Generic Repository
        var recipeEntity = _mapper.Map<Recipe>(dto);
        recipeEntity.Rating = 0;
        recipeEntity.CreatedAt = DateTime.UtcNow;

        await _recipeRepository.AddAsync(recipeEntity);
        // Після AddAsync entity вже має Id (EF Core заповнює його)

        // 2. Якщо є вибрані інгредієнти — зберігаємо зв'язки
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

        // 3. Повертаємо рецепт з інгредієнтами через DbContext
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