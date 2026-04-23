namespace WhatToCook.MAUI.Models
{
    public class RecipeModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CookTimeMinutes { get; set; }

        public string MoodTag { get; set; } = string.Empty;

        public double Rating { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}