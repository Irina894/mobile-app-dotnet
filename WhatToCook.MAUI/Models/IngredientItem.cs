using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace WhatToCook.MAUI.Models;

public class IngredientItem : INotifyPropertyChanged
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonIgnore]
    private bool _isSelected;

    [JsonIgnore]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectionBorderColor));
            OnPropertyChanged(nameof(SelectionBackground));
            OnPropertyChanged(nameof(ChipTextColor));
        }
    }

    // Колір рамки чіпса
    [JsonIgnore]
    public string SelectionBorderColor =>
        IsSelected ? "#1E40AF" : "#E5E7EB";

    // Фон чіпса
    [JsonIgnore]
    public string SelectionBackground =>
        IsSelected ? "#EFF6FF" : "White";

    // Колір тексту чіпса
    [JsonIgnore]
    public string ChipTextColor =>
        IsSelected ? "#1E40AF" : "#374151";

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}