namespace WhatToCook.BLL.DTOs.Recipe;

public class RecipeIngredientDto
{
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}