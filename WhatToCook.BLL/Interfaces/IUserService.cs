using WhatToCook.BLL.DTOs.User;
namespace WhatToCook.BLL.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserDto dto);
    Task<UserDto?> GetByIdAsync(int id);
}