using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.Interfaces;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAll()
    {
        var recipes = await _recipeService.GetAllRecipesAsync();
        return Ok(recipes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetById(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);

        if (recipe == null)
        {
            return NotFound();
        }

        return Ok(recipe);
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Create([FromBody] CreateRecipeDto dto)
    {
        if (dto == null)
        {
            return BadRequest();
        }

        var result = await _recipeService.CreateRecipeAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecipeDto dto)
    {
        var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);

        if (existingRecipe == null)
        {
            return NotFound();
        }

        await _recipeService.UpdateRecipeAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);

        if (existingRecipe == null)
        {
            return NotFound();
        }

        await _recipeService.DeleteRecipeAsync(id);
        return NoContent();
    }
}