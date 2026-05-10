using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;

using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels;

// ── Моделі для деталей ────────────────────────────────────────────────────

public class RecipeIngredientDetail
{
    [JsonPropertyName("ingredientId")]
    public int IngredientId { get; set; }

    [JsonPropertyName("ingredientName")]
    public string IngredientName { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;
}

// Розширює базову Recipe — додає список інгредієнтів що приходить з API
public class RecipeDetailModel : RecipeModel
{
    [JsonPropertyName("ingredients")]
    public List<RecipeIngredientDetail> Ingredients { get; set; } = new();
}

public class RecipeStep
{
    public int Number { get; set; }
    public string Description { get; set; } = string.Empty;
    public string NumberLabel => $"{Number}";
}

// ── ViewModel ─────────────────────────────────────────────────────────────

[QueryProperty(nameof(RecipeId), "id")]
public class RecipeDetailViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService _recipeApiService;

    // Shell передає id через навігацію: GoToAsync("recipedetail?id=5")
    private int _recipeId;
    public int RecipeId
    {
        get => _recipeId;
        set
        {
            _recipeId = value;
            OnPropertyChanged();
            _ = LoadRecipeAsync(value);
        }
    }

    private RecipeModel? _recipe;
    public RecipeModel? Recipe
    {
        get => _recipe;
        set { _recipe = value; OnPropertyChanged(); }
    }

    // Тільки інгредієнти ЦЬОГО рецепту — приходять з api/recipes/{id}
    public ObservableCollection<RecipeIngredientDetail> RecipeIngredients { get; } = new();

    public ObservableCollection<RecipeStep> Steps { get; } = new();

    private bool _isLoading = true;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set { _hasError = value; OnPropertyChanged(); }
    }

    private bool _hasIngredients;
    public bool HasIngredients
    {
        get => _hasIngredients;
        set { _hasIngredients = value; OnPropertyChanged(); }
    }

    public ICommand BackCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand RetryCommand { get; }

    public RecipeDetailViewModel(IRecipeApiService recipeApiService)
    {
        _recipeApiService = recipeApiService;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        ToggleFavoriteCommand = new Command(async () =>
        {
            if (Recipe == null) return;
            Recipe.IsFavorite = !Recipe.IsFavorite;
            await _recipeApiService.UpdateAsync(Recipe);
            OnPropertyChanged(nameof(Recipe));
        });

        RetryCommand = new Command(async () => await LoadRecipeAsync(_recipeId));
    }

    private async Task LoadRecipeAsync(int id)
    {
        if (id <= 0) return;

        try
        {
            IsLoading = true;
            HasError = false;

            // Отримуємо розширену модель з інгредієнтами
            var detail = await _recipeApiService.GetByIdWithIngredientsAsync(id);

            if (detail == null)
            {
                HasError = true;
                return;
            }

            // Базові поля рецепту
            Recipe = detail;

            // Інгредієнти саме цього рецепту
            RecipeIngredients.Clear();
            foreach (var ingredient in detail.Ingredients)
                RecipeIngredients.Add(ingredient);

            HasIngredients = RecipeIngredients.Count > 0;

            // Покрокова інструкція
            GenerateSteps(detail);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading recipe detail: {ex.Message}");
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void GenerateSteps(RecipeModel recipe)
    {
        Steps.Clear();

        var steps = recipe.Category switch
        {
            "Soup" => new[]
            {
                "Prepare all vegetables: wash, peel and cut into medium pieces.",
                "Heat oil in a large pot. Sauté onions until golden.",
                "Add remaining vegetables and cook for 5 minutes.",
                "Pour in broth, bring to a boil.",
                $"Reduce heat and simmer for {Math.Max(5, recipe.CookTimeMinutes - 10)} minutes.",
                "Season with salt, pepper and herbs. Serve hot."
            },
            "Pasta" => new[]
            {
                "Bring a large pot of salted water to a boil.",
                "Cook pasta until al dente per package instructions.",
                "Meanwhile, prepare the sauce in a separate pan.",
                "Drain pasta, reserving 1 cup of cooking water.",
                "Toss pasta with sauce, adding water as needed.",
                "Serve immediately with grated cheese."
            },
            "Breakfast" => new[]
            {
                "Bring all ingredients to room temperature.",
                "Heat pan over medium heat with butter or oil.",
                "Mix ingredients in a bowl until combined.",
                $"Cook {Math.Max(2, recipe.CookTimeMinutes / 2)} minutes on first side.",
                "Flip carefully, cook 2-3 minutes more.",
                "Serve hot with your favorite toppings."
            },
            "Salad" => new[]
            {
                "Wash and thoroughly dry all vegetables.",
                "Cut everything into bite-sized pieces.",
                "Whisk together oil, vinegar, salt and pepper.",
                "Combine all vegetables in a large bowl.",
                "Drizzle with dressing and toss gently.",
                "Serve immediately or chill for up to 1 hour."
            },
            _ => new[]
            {
                "Prepare all ingredients: wash, cut and measure before starting.",
                "Preheat your cooking surface to the right temperature.",
                "Start with ingredients that take longest to cook.",
                $"Cook for {Math.Max(5, recipe.CookTimeMinutes / 2)} minutes, checking regularly.",
                "Adjust seasoning — salt, pepper or herbs as needed.",
                "Rest 2-3 minutes before serving. Enjoy!"
            }
        };

        for (int i = 0; i < steps.Length; i++)
            Steps.Add(new RecipeStep { Number = i + 1, Description = steps[i] });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}