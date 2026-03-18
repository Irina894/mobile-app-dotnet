using AutoMapper;
using WhatToCook.BLL.DTOs.Ingredient;
using WhatToCook.BLL.Interfaces;
using WhatToCook.DAL.Entities;
using WhatToCook.DAL.Repositories;

namespace WhatToCook.BLL.Services;

public class IngredientService : IIngredientService
{
    private readonly IRepository<Ingredient> _repository;
    private readonly IMapper _mapper;

    public IngredientService(IRepository<Ingredient> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IngredientDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<IngredientDto>>(await _repository.GetAllAsync());

    public async Task<IngredientDto> CreateAsync(IngredientDto dto)
    {
        var entity = _mapper.Map<Ingredient>(dto);
        await _repository.AddAsync(entity);
        return _mapper.Map<IngredientDto>(entity);
    }
}