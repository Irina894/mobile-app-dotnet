using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatToCook.DAL.Data;

namespace WhatToCook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDatabaseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestDatabaseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                // Спроба отримати кількість рецептів (навіть якщо 0)
                var count = await _context.Recipes.CountAsync();
                return Ok(new { Message = "З'єднання встановлено!", RecipesCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Помилка підключення", Error = ex.Message });
            }
        }
    }
}