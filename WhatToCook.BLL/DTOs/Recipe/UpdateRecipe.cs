using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WhatToCook.BLL.DTOs.Recipe;

public class UpdateRecipeDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CookTimeMinutes { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
