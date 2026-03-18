using AutoMapper;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IRepository<FavoriteRecipe> _favoriteRepo;
    private readonly IMapper _mapper;

    public FavoriteService(IRepository<FavoriteRecipe> favoriteRepo, IMapper mapper)
    {
        _favoriteRepo = favoriteRepo;
        _mapper = mapper;
    }

    public async Task AddToFavoriteAsync(int userId, int recipeId)
    {
        await _favoriteRepo.AddAsync(new FavoriteRecipe { UserId = userId, RecipeId = recipeId });
    }

    public async Task<IEnumerable<FavoriteRecipeDto>> GetUserFavoritesAsync(int userId)
    {
        var favorites = await _favoriteRepo.GetAllAsync();
        var userFavorites = favorites.Where(f => f.UserId == userId);
        return _mapper.Map<IEnumerable<FavoriteRecipeDto>>(userFavorites);
    }
}