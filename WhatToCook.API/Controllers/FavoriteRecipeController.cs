using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;

namespace WhatToCook.API.Controllers
{
    
    [ApiController] // Обов'язково для Swagger 
    [Route("api/[controller]")] // Шлях api/FavoriteRecipe 
    public class FavoriteRecipeController : ControllerBase // Успадковуємо ControllerBase [cite: 600]
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteRecipeController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet("user/{userId}")] // Отримати всі обрані користувача [cite: 599]
        public async Task<ActionResult<IEnumerable<FavoriteRecipeDto>>> GetUserFavorites(int userId)
        {
            var result = await _favoriteService.GetUserFavoritesAsync(userId);
            return Ok(result);
        }

        [HttpPost] // Додати в обране [cite: 599]
        public async Task<ActionResult<FavoriteRecipeDto>> Add(CreateFavoriteRecipeDto dto)
        {
            var result = await _favoriteService.AddToFavoritesAsync(dto);
            return Ok(result);
        }

        [HttpDelete("user/{userId}/recipe/{recipeId}")] // Видалити [cite: 599]
        public async Task<IActionResult> Remove(int userId, int recipeId)
        {
            await _favoriteService.RemoveFromFavoritesAsync(userId, recipeId);
            return NoContent();
        }
    }
}