using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.DTOs.User;
using WhatToCook.BLL.Interfaces;

namespace WhatToCook.API.Controllers;

[ApiController] // Вимога для Swagger [cite: 598]
[Route("api/[controller]")] // Маршрут: api/users [cite: 598]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")] // GET api/users/5
    public async Task<ActionResult<UserDto>> Get(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound(); // Повертаємо 404 [cite: 599, 602]
        return Ok(user); // Повертаємо 200 OK 
    }

    [HttpPost("register")] // POST api/users/register
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto dto)
    {
        var result = await _userService.RegisterAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")] // DELETE api/users/5
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteAsync(id);
        return NoContent(); // Повертаємо 204 
    }
}