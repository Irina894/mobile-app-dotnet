using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatToCook.BLL.DTOs.Ingredient
{

    public class UpdateIngredientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Calories { get; set; }
        public bool IsAllergen { get; set; }
    }
}
