using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatToCook.BLL.DTOs.Ingredient
{
    public class CreateIngredientDto
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
    }
}
