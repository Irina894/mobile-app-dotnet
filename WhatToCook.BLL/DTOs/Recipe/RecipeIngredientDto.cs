namespace WhatToCook.BLL.DTOs.Recipe;

public class RecipeIngredientDto
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }      // потрібно для UI
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}