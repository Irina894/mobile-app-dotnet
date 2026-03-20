using AutoMapper;
using WhatToCook.BLL.DTOs.User;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepo;
    private readonly IMapper _mapper;

    public UserService(IRepository<User> userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
    }

    // 1. Реалізація GetAllAsync (Виправляє помилку GetAllAsync)
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    { var users = await _userRepo.GetAllAsync(); 
         return _mapper.Map<IEnumerable<UserDto>>(users); 
    }

    // 2. Реалізація GetByIdAsync
    public async Task<UserDto?> GetByIdAsync(int id)
    {
     var user = await _userRepo.GetByIdAsync(id); 
         return _mapper.Map<UserDto>(user); 
    }

    // 3. Реалізація RegisterAsync
    public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
    {
        var userEntity = _mapper.Map<User>(dto); 
       await _userRepo.AddAsync(userEntity);
        return _mapper.Map<UserDto>(userEntity); 
    }

    // 4. Реалізація UpdateAsync (Виправляє помилку UpdateAsync)
    public async Task UpdateAsync(int id, UserDto dto)
    {
       var existingUser = await _userRepo.GetByIdAsync(id); 
        if (existingUser != null)
        {
            // Мапимо дані з DTO в існуючу сутність
          _mapper.Map(dto, existingUser); 
           await _userRepo.UpdateAsync(existingUser); 
        }
    }

    // 5. Реалізація DeleteAsync (Виправляє помилку DeleteAsync)
    public async Task DeleteAsync(int id)
    {
        await _userRepo.DeleteAsync(id); 
    }
}