using AutoMapper;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

public class FavoriteService : IFavoriteService
{
    private readonly IRepository<FavoriteRecipe> _favoriteRepo;
    private readonly IMapper _mapper;

    public FavoriteService(IRepository<FavoriteRecipe> favoriteRepo, IMapper mapper)
    {
        _favoriteRepo = favoriteRepo;
        _mapper = mapper;
    }

    public async Task<FavoriteRecipeDto> AddToFavoritesAsync(CreateFavoriteRecipeDto dto)
    {
        var favoriteEntity = _mapper.Map<FavoriteRecipe>(dto);
        await _favoriteRepo.AddAsync(favoriteEntity); // Додавання через репозиторій [cite: 121]
        return _mapper.Map<FavoriteRecipeDto>(favoriteEntity);
    }

    public async Task<IEnumerable<FavoriteRecipeDto>> GetUserFavoritesAsync(int userId)
    {
        var favorites = await _favoriteRepo.GetAllAsync(); // Отримання всіх [cite: 108]
        var userFavorites = favorites.Where(f => f.UserId == userId);
        return _mapper.Map<IEnumerable<FavoriteRecipeDto>>(userFavorites);
    }

    public async Task RemoveFromFavoritesAsync(int userId, int recipeId)
    {
        var favorites = await _favoriteRepo.GetAllAsync();
        var favorite = favorites.FirstOrDefault(f => f.UserId == userId && f.RecipeId == recipeId);
        if (favorite != null)
        {
            await _favoriteRepo.DeleteAsync(favorite.Id); // Видалення за ID [cite: 114]
        }
    }
}