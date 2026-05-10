namespace WhatToCook.DAL.Entities;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Calories { get; set; }
    public bool IsAllergen { get; set; }

    // Додаємо це поле для відповідності UI колеги
    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}