using WhatToCook.BLL.DTOs.User;
namespace WhatToCook.BLL.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(); // Вимога ТЗ: 5 методів
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> RegisterAsync(RegisterUserDto dto);
    Task UpdateAsync(int id, UserDto dto);
    Task DeleteAsync(int id);
}