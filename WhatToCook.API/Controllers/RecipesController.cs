using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.Interfaces;
using WhatToCook.BLL.DTOs.Recipe;

namespace WhatToCook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    // Це ваш існуючий GET (Отримати всі)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var recipes = await _recipeService.GetAllRecipesAsync();
        return Ok(recipes);
    }

    // ДОДАЄМО ЦЕЙ БЛОК (POST - Створити)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecipeDto dto)
    {
        if (dto == null) return BadRequest();

        var result = await _recipeService.CreateRecipeAsync(dto);
        return Ok(result);
    }
}