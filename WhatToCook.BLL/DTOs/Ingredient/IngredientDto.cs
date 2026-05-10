using System.Text.Json.Serialization;

namespace WhatToCook.BLL.DTOs.Ingredient;

public class IngredientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Calories { get; set; }
    public bool IsAllergen { get; set; }

    [JsonPropertyName("imageUrl")] // ← має точно збігатись з JSON
    public string ImageUrl { get; set; } = string.Empty;

}