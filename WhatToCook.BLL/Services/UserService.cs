using AutoMapper;
using WhatToCook.BLL.DTOs.User;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repository;
    private readonly IMapper _mapper;

    public UserService(IRepository<User> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.PasswordHash = dto.Password; // Тимчасово
        await _repository.AddAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByIdAsync(int id) =>
        _mapper.Map<UserDto>(await _repository.GetByIdAsync(id));
}