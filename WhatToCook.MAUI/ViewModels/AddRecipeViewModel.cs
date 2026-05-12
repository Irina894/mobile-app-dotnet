using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;

namespace WhatToCook.MAUI.ViewModels;

public class AddRecipeViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService _recipeApiService;
    private WhatToCook.MAUI.Models.Recipe _recipe = new();
    private string _imagePreview = string.Empty;

    // ── Поля форми (без змін) ─────────────────────────────────────────────
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
        set { if (int.TryParse(value, out int r)) _recipe.CookTimeMinutes = r; OnPropertyChanged(); }
    }
    public string Servings
    {
        get => _recipe.Servings.ToString();
        set { if (int.TryParse(value, out int r)) _recipe.Servings = r; OnPropertyChanged(); }
    }
    public string Difficulty
    {
        get => _recipe.Difficulty;
        set { _recipe.Difficulty = value; OnPropertyChanged(); }
    }
    public string Category
    {
        get => _recipe.Category;
        set { _recipe.Category = value; OnPropertyChanged(); UpdateAccentColors(); }
    }
    public string ImageUrl
    {
        get => _recipe.ImageUrl;
        set { _recipe.ImageUrl = value; OnPropertyChanged(); ImagePreview = value; }
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

    // ── Інгредієнти ───────────────────────────────────────────────────────
    // Повний список з API (відображається в UI)
    public ObservableCollection<IngredientItem> AvailableIngredients { get; } = new();

    // Кількість вибраних — для лічильника в UI
    private int _selectedCount;
    public int SelectedIngredientsCount
    {
        get => _selectedCount;
        set { _selectedCount = value; OnPropertyChanged(); OnPropertyChanged(nameof(SelectedCountText)); }
    }
    public string SelectedCountText => _selectedCount == 0
        ? "None selected"
        : $"{_selectedCount} selected";

    // ── Picker списки ─────────────────────────────────────────────────────
    public ObservableCollection<string> DifficultyOptions { get; } = new()
        { "Easy", "Medium", "Hard" };
    public ObservableCollection<string> CategoryOptions { get; } = new()
        { "Breakfast", "Lunch", "Dinner", "Dessert", "Soup", "Salad", "Pasta", "Meat", "Vegetarian", "Vegan" };

    // ── Команди ───────────────────────────────────────────────────────────
    public ICommand SaveCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand PickImageCommand { get; }
    public ICommand ToggleIngredientCommand { get; }

    public AddRecipeViewModel(IRecipeApiService recipeApiService)
    {
        _recipeApiService = recipeApiService;

        SaveCommand = new Command(async () => await OnSave());
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        PickImageCommand = new Command(OnPickImage);
        ToggleIngredientCommand = new Command<IngredientItem>(OnToggleIngredient);

        Difficulty = "Medium";
        Category = "Dinner";
        CookTimeMinutes = "30";
        Servings = "4";
        UpdateAccentColors();

        _ = LoadIngredientsAsync();
    }

    private async Task LoadIngredientsAsync()
    {
        try
        {
            var ingredients = await _recipeApiService.GetAllIngredientsAsync();
            AvailableIngredients.Clear();
            foreach (var item in ingredients)
                AvailableIngredients.Add(item);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading ingredients: {ex.Message}");
        }
    }

    private void OnToggleIngredient(IngredientItem? ingredient)
    {
        if (ingredient == null) return;
        ingredient.IsSelected = !ingredient.IsSelected;
        SelectedIngredientsCount = AvailableIngredients.Count(i => i.IsSelected);
    }

    private async Task OnSave()
    {
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

        _recipe.CreatedAt = DateTime.Now;
        _recipe.Rating = 0;
        _recipe.MoodTag = Category;
        _recipe.IsMyRecipe = true;

        // Збираємо Id вибраних інгредієнтів
        var selectedIds = AvailableIngredients
            .Where(i => i.IsSelected)
            .Select(i => i.Id)
            .ToList();

        try
        {
            // Надсилаємо рецепт + ingredientIds одним запитом
            var payload = new
            {
                title = _recipe.Title,
                description = _recipe.Description,
                cookTimeMinutes = _recipe.CookTimeMinutes,
                category = _recipe.Category,
                difficulty = _recipe.Difficulty,
                servings = _recipe.Servings,
                imageUrl = _recipe.ImageUrl,
                accentColor = _recipe.AccentColor,
                accentTextColor = _recipe.AccentTextColor,
                isMyRecipe = _recipe.IsMyRecipe,
                ingredientIds = selectedIds          // ← нове поле
            };

            var result = await _recipeApiService.CreateWithIngredientsAsync(payload);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Success",
                    $"Recipe '{Title}' saved with {selectedIds.Count} ingredient(s)!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Could not save the recipe to the server.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
    }

    private async void OnPickImage()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Please select a recipe photo"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();

                // 3. Відправляємо на сервер
                var uploadedUrl = await _recipeApiService.UploadImageAsync(stream, result.FileName);

                if (!string.IsNullOrEmpty(uploadedUrl))
                {
                    ImageUrl = uploadedUrl;
                    ImagePreview = uploadedUrl;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to upload image to server.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error picking image: {ex.Message}", "OK");
        }
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}