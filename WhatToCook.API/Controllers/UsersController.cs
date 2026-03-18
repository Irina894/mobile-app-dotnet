using Microsoft.AspNetCore.Mvc;
using WhatToCook.BLL.DTOs.User;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterUserDto dto)
    {
        // Логіка реєстрації (ваша частина BLL)
        return Ok("Користувач успішно зареєстрований");
    }

    [HttpGet("{id}")]
    public IActionResult GetProfile(int id) => Ok(new UserDto { Id = id, Name = "Студент ЧНУ" });
}