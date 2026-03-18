using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.DTOs.Ingredient;
// Тут ми додамо сервіс інгредієнтів, коли ви його створите, 
// а поки що зробимо базовий каркас
[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    // Поки що тут будуть пусті методи, щоб Swagger їх просто показав
    [HttpGet]
    public IActionResult GetAll() => Ok(new List<IngredientDto>());

    [HttpPost]
    public IActionResult Create([FromBody] IngredientDto dto) => Ok(dto);
}