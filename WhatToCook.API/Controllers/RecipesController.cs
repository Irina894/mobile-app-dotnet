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
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAll([FromQuery] int? userId = null)
    {
        var recipes = userId.HasValue
            ? await _recipeService.GetAllRecipesAsync(userId.Value)
            : await _recipeService.GetAllRecipesAsync();
        return Ok(recipes);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> Search(
        [FromQuery] string? ids = null,
        [FromQuery] string? q = null,
        [FromQuery] int? userId = null)
    {
        var ingredientIds = new List<int>();
        if (!string.IsNullOrWhiteSpace(ids))
        {
            ingredientIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : 0)
                .Where(id => id > 0).ToList();
        }

        var results = userId.HasValue
            ? await _recipeService.SearchRecipesAsync(ingredientIds, q, userId.Value)
            : await _recipeService.SearchRecipesAsync(ingredientIds, q);
        return Ok(results);
    }


    // GET api/recipes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetById(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }

    // POST api/recipes
    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Create([FromBody] CreateRecipeDto dto)
    {
        if (dto == null) return BadRequest("Request body is empty.");
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required.");

        var result = await _recipeService.CreateRecipeAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

   
    // PUT api/recipes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecipeDto dto)
    {
        var existing = await _recipeService.GetRecipeByIdAsync(id);
        if (existing == null) return NotFound();
        await _recipeService.UpdateRecipeAsync(id, dto);
        return NoContent();
    }

    // DELETE api/recipes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _recipeService.GetRecipeByIdAsync(id);
        if (existing == null) return NotFound();
        await _recipeService.DeleteRecipeAsync(id);
        return NoContent();
    }
}