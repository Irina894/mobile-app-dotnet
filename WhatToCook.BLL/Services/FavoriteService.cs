using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Data;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IRepository<FavoriteRecipe> _favoriteRepo;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FavoriteService(
        IRepository<FavoriteRecipe> favoriteRepo,
        AppDbContext context,
        IMapper mapper)
    {
        _favoriteRepo = favoriteRepo;
        _context = context;
        _mapper = mapper;
    }

    public async Task<FavoriteRecipeDto> AddToFavoritesAsync(CreateFavoriteRecipeDto dto)
    {
        // Перевіряємо, чи вже є — щоб не дублювати
        var existing = await _context.FavoriteRecipes
            .FirstOrDefaultAsync(f => f.UserId == dto.UserId && f.RecipeId == dto.RecipeId);

        if (existing != null)
            return _mapper.Map<FavoriteRecipeDto>(existing);

        var favoriteEntity = _mapper.Map<FavoriteRecipe>(dto);
        favoriteEntity.AddedAt = DateTime.UtcNow;
        await _favoriteRepo.AddAsync(favoriteEntity);
        return _mapper.Map<FavoriteRecipeDto>(favoriteEntity);
    }

    public async Task<IEnumerable<FavoriteRecipeDto>> GetUserFavoritesAsync(int userId)
    {
        var favorites = await _context.FavoriteRecipes
            .Include(f => f.Recipe)
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<FavoriteRecipeDto>>(favorites);
    }

    public async Task<IEnumerable<RecipeDto>> GetUserFavoriteRecipesAsync(int userId)
    {
        var recipes = await _context.FavoriteRecipes
            .Include(f => f.Recipe)
                .ThenInclude(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.AddedAt)
            .Select(f => f.Recipe)
            .ToListAsync();

        var dtos = _mapper.Map<List<RecipeDto>>(recipes);
        // Всі вони — улюблені за визначенням
        foreach (var dto in dtos)
            dto.IsFavorite = true;
        return dtos;
    }

    public async Task RemoveFromFavoritesAsync(int userId, int recipeId)
    {
        var favorite = await _context.FavoriteRecipes
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        if (favorite != null)
            await _favoriteRepo.DeleteAsync(favorite.Id);
    }

    public async Task<bool> ToggleAsync(int userId, int recipeId)
    {
        var existing = await _context.FavoriteRecipes
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);

        if (existing != null)
        {
            await _favoriteRepo.DeleteAsync(existing.Id);
            return false; // більше не в обраному
        }

        await _favoriteRepo.AddAsync(new FavoriteRecipe
        {
            UserId = userId,
            RecipeId = recipeId,
            AddedAt = DateTime.UtcNow
        });
        return true; // тепер в обраному
    }

    public async Task<bool> IsFavoriteAsync(int userId, int recipeId)
    {
        return await _context.FavoriteRecipes
            .AsNoTracking()
            .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);
    }

    public async Task<HashSet<int>> GetFavoriteRecipeIdsAsync(int userId)
    {
        var ids = await _context.FavoriteRecipes
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => f.RecipeId)
            .ToListAsync();
        return new HashSet<int>(ids);
    }
}