using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.DTOs.Ingredient;
using WhatToCook.BLL.Interfaces;
// Тут ми додамо сервіс інгредієнтів, коли ви його створите, 
// а поки що зробимо базовий каркас
[ApiController]
[Route("api/[controller]")]

public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientsController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAll()
    {
        var ingredients = await _ingredientService.GetAllIngredientsAsync();
        return Ok(ingredients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IngredientDto>> GetById(int id)
    {
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id);

        if (ingredient == null)
        {
            return NotFound();
        }

        return Ok(ingredient);
    }

    [HttpPost]
    public async Task<ActionResult<IngredientDto>> Create([FromBody] CreateIngredientDto dto)
    {
        if (dto == null)
        {
            return BadRequest();
        }

        var result = await _ingredientService.CreateIngredientAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIngredientDto dto)
    {
        var existingIngredient = await _ingredientService.GetIngredientByIdAsync(id);

        if (existingIngredient == null)
        {
            return NotFound();
        }

        await _ingredientService.UpdateIngredientAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingIngredient = await _ingredientService.GetIngredientByIdAsync(id);

        if (existingIngredient == null)
        {
            return NotFound();
        }

        await _ingredientService.DeleteIngredientAsync(id);
        return NoContent();
    }
}
