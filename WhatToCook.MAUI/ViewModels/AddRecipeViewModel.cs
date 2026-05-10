using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces; // Додано для роботи з сервісом

namespace WhatToCook.MAUI.ViewModels;

public class AddRecipeViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService _recipeApiService; // Додано поле сервісу
    private WhatToCook.MAUI.Models.Recipe _recipe = new();
    private string _imagePreview = string.Empty;

    public string Title
    {
        get => _recipe.Title;
        set { _recipe.Title = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => _recipe.Description;
        set { _recipe.Description = value; OnPropertyChanged(); }
    }

    public string CookTimeMinutes
    {
        get => _recipe.CookTimeMinutes.ToString();
        set
        {
            if (int.TryParse(value, out int result))
                _recipe.CookTimeMinutes = result;
            OnPropertyChanged();
        }
    }

    public string Servings
    {
        get => _recipe.Servings.ToString();
        set
        {
            if (int.TryParse(value, out int result))
                _recipe.Servings = result;
            OnPropertyChanged();
        }
    }

    public string Difficulty
    {
        get => _recipe.Difficulty;
        set { _recipe.Difficulty = value; OnPropertyChanged(); }
    }

    public string Category
    {
        get => _recipe.Category;
        set
        {
            _recipe.Category = value;
            OnPropertyChanged();
            UpdateAccentColors();
        }
    }

    public string ImageUrl
    {
        get => _recipe.ImageUrl;
        set
        {
            _recipe.ImageUrl = value;
            OnPropertyChanged();
            ImagePreview = value;
        }
    }

    public string ImagePreview
    {
        get => _imagePreview;
        set { _imagePreview = value; OnPropertyChanged(); }
    }

    public bool IsMyRecipe
    {
        get => _recipe.IsMyRecipe;
        set { _recipe.IsMyRecipe = value; OnPropertyChanged(); }
    }

    public ObservableCollection<string> DifficultyOptions { get; } = new()
    {
        "Easy", "Medium", "Hard"
    };

    public ObservableCollection<string> CategoryOptions { get; } = new()
    {
        "Breakfast", "Lunch", "Dinner", "Dessert", "Soup", "Salad", "Pasta", "Meat", "Vegetarian", "Vegan"
    };

    public ICommand SaveCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand PickImageCommand { get; }

    // Конструктор тепер приймає сервіс через Dependency Injection
    public AddRecipeViewModel(IRecipeApiService recipeApiService)
    {
        _recipeApiService = recipeApiService;

        SaveCommand = new Command(OnSave);
        BackCommand = new Command(OnBack);
        PickImageCommand = new Command(OnPickImage);

        // Значення за замовчуванням
        Difficulty = "Medium";
        Category = "Dinner";
        CookTimeMinutes = "30";
        Servings = "4";

        UpdateAccentColors();
    }

    private void UpdateAccentColors()
    {
        _recipe.AccentColor = Category switch
        {
            "Breakfast" => "#B45309",
            "Soup" => "#9F1239",
            "Pasta" => "#92400E",
            "Salad" => "#14532D",
            "Meat" => "#7C2D12",
            "Dessert" => "#86198F",
            _ => "#1E40AF"
        };

        _recipe.AccentTextColor = Category switch
        {
            "Breakfast" => "#92400E",
            "Soup" => "#881337",
            "Pasta" => "#78350F",
            "Salad" => "#166534",
            "Meat" => "#6B2110",
            "Dessert" => "#701A75",
            _ => "#1E3A8A"
        };
    }

    private async void OnPickImage()
    {
        await Shell.Current.DisplayAlert("Coming Soon",
            "Image picker will be implemented with proper permissions", "OK");
    }

    private async void OnSave()
    {
        // 1. Валідація обов'язкових полів
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a recipe title", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(CookTimeMinutes) || CookTimeMinutes == "0")
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a valid cook time", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Servings) || Servings == "0")
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a valid number of servings", "OK");
            return;
        }

        // 2. Підготовка об'єкта моделі
        _recipe.CreatedAt = DateTime.Now;
        _recipe.Rating = 0;
        _recipe.MoodTag = Category;
        _recipe.IsMyRecipe = true; // Позначаємо, що це власний рецепт користувача

        try
        {
            // 3. Реалізація збереження в базу через API
            var result = await _recipeApiService.CreateAsync(_recipe);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Success", $"Recipe '{Title}' has been saved to the database!", "OK");

                // 4. Повернення назад після успішного збереження
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Could not save the recipe to the server.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}