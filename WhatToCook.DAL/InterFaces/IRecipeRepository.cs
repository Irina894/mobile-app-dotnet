using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WhatToCook.DAL.Entities;

namespace WhatToCook.DAL.Interfaces;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<Recipe?> GetByIdAsync(int id);
    Task AddAsync(Recipe recipe);
    Task SaveChangesAsync();
}

