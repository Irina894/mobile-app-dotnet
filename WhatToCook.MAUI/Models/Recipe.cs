using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization; // Обов'язково для мапінгу JSON

namespace WhatToCook.MAUI.Models;

public class Recipe : INotifyPropertyChanged
{
    // === ДАНІ З СЕРВЕРА ===
    // Атрибути JsonPropertyName жорстко кажуть застосунку: 
    // "Коли бачиш у JSON поле 'imageUrl', клади його сюди!"

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("cookTimeMinutes")]
    public int CookTimeMinutes { get; set; }

    [JsonPropertyName("moodTag")]
    public string MoodTag { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("imageUrl")] // ГОЛОВНИЙ ФІКС ДЛЯ КАРТИНОК
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonPropertyName("servings")]
    public int Servings { get; set; } = 2;

    [JsonPropertyName("accentColor")]
    public string AccentColor { get; set; } = "#1E40AF";

    [JsonPropertyName("accentTextColor")]
    public string AccentTextColor { get; set; } = "#1E3A8A";

    [JsonPropertyName("isMyRecipe")]
    public bool IsMyRecipe { get; set; }

    [JsonIgnore]
    private bool _isFavorite;

    [JsonIgnore]
    public bool IsFavorite
    {
        get => _isFavorite;
        set { _isFavorite = value; OnPropertyChanged(); OnPropertyChanged(nameof(FavoriteIcon)); }
    }

    [JsonIgnore]
    public string RatingText => Rating > 0 ? $"⭐ {Rating:F1}" : "New";

    [JsonIgnore]
    public string FavoriteIcon => IsFavorite ? "heart_menu.png" : "heart.svg";

    [JsonIgnore]
    public string DifficultyLabel => Difficulty;

    [JsonIgnore]
    public string ServingsLabel => $"{Servings} servings";

    [JsonIgnore]
    public string ShortDescription => Description.Length > 80
        ? Description[..80] + "…"
        : Description;


    // === ОНОВЛЕННЯ ІНТЕРФЕЙСУ ===
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}