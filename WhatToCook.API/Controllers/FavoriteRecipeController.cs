using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;

namespace WhatToCook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteRecipeController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteRecipeController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    // GET api/FavoriteRecipe/user/1
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<FavoriteRecipeDto>>> GetUserFavorites(int userId)
        => Ok(await _favoriteService.GetUserFavoritesAsync(userId));

    // GET api/FavoriteRecipe/user/1/recipes  ← повертає RecipeDto, зручніше для UI
    [HttpGet("user/{userId}/recipes")]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetUserFavoriteRecipes(int userId)
        => Ok(await _favoriteService.GetUserFavoriteRecipesAsync(userId));

    [HttpPost]
    public async Task<ActionResult<FavoriteRecipeDto>> Add(CreateFavoriteRecipeDto dto)
        => Ok(await _favoriteService.AddToFavoritesAsync(dto));

    // POST api/FavoriteRecipe/toggle?userId=1&recipeId=5
    [HttpPost("toggle")]
    public async Task<ActionResult<object>> Toggle([FromQuery] int userId, [FromQuery] int recipeId)
    {
        var isFav = await _favoriteService.ToggleAsync(userId, recipeId);
        return Ok(new { isFavorite = isFav });
    }

    [HttpDelete("user/{userId}/recipe/{recipeId}")]
    public async Task<IActionResult> Remove(int userId, int recipeId)
    {
        await _favoriteService.RemoveFromFavoritesAsync(userId, recipeId);
        return NoContent();
    }
}