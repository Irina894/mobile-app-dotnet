using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;

using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels;

public class FavoritesViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService? _recipeApiService;

    private ObservableCollection<RecipeModel> _allFavorites;
    private ObservableCollection<RecipeModel> _filteredFavorites;
    private string _searchQuery = string.Empty;

    // ── Колекції ──────────────────────────────────────────────────────────
    public ObservableCollection<RecipeModel> FilteredFavorites
    {
        get => _filteredFavorites;
        set { _filteredFavorites = value; OnPropertyChanged(); }
    }

    // Кружечки інгредієнтів (підвантажуються з API)
    public ObservableCollection<IngredientItem> Ingredients { get; } = new();

    // ── Пошук за текстом ──────────────────────────────────────────────────
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSearchQuery));
            _ = ApplyFiltersAsync();
        }
    }

    public bool HasSearchQuery => !string.IsNullOrEmpty(_searchQuery);

    // ── Стани UI ──────────────────────────────────────────────────────────
    private int _selectedIngredientsCount;
    public bool ShowFilterBadge => _selectedIngredientsCount > 0;
    public string FilterBadgeText => _selectedIngredientsCount == 1
        ? "1 ingredient selected"
        : $"{_selectedIngredientsCount} ingredients selected";

    public bool HasActiveFilters =>
        _selectedIngredientsCount > 0 || !string.IsNullOrWhiteSpace(_searchQuery);

    public string FavoritesCountText => _allFavorites.Count == 0
        ? "No favorites yet"
        : $"{_allFavorites.Count} favorite recipe{(_allFavorites.Count == 1 ? "" : "s")}";

    public string RecipesSectionTitle => HasActiveFilters
        ? "Search results"
        : "All favorites";

    public bool IsEmpty => _allFavorites.Count == 0;          // Зовсім нема улюблених
    public bool IsNotEmpty => _allFavorites.Count > 0;
    public bool IsResultEmpty => _allFavorites.Count > 0 && _filteredFavorites.Count == 0; // Є улюблені, але фільтр нічого не дав

    // ── Команди ───────────────────────────────────────────────────────────
    public ICommand SearchCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand ToggleIngredientCommand { get; }
    public ICommand RemoveFavoriteCommand { get; }
    public ICommand ViewRecipeCommand { get; }
    public ICommand BrowseRecipesCommand { get; }
    public ICommand BackCommand { get; }

    // ── Конструктор за замовчуванням (XAML preview / без DI) ──────────────
    public FavoritesViewModel() : this(null) { }

    // ── Конструктор з DI ──────────────────────────────────────────────────
    public FavoritesViewModel(IRecipeApiService? recipeApiService)
    {
        _recipeApiService = recipeApiService;

        _allFavorites = new ObservableCollection<RecipeModel>();
        LoadSampleFavorites(); // Поки тримаємо sample-дані для UI
        _filteredFavorites = new ObservableCollection<RecipeModel>(_allFavorites);

        SearchCommand = new Command(async () => await ApplyFiltersAsync());
        ClearSearchCommand = new Command(() => SearchQuery = string.Empty);
        ClearFiltersCommand = new Command(async () => await ClearAllFiltersAsync());
        ToggleIngredientCommand = new Command<IngredientItem>(async i => await OnToggleIngredientAsync(i));
        RemoveFavoriteCommand = new Command<RecipeModel>(async r => await RemoveFavoriteAsync(r));

        ViewRecipeCommand = new Command<RecipeModel>(async recipe =>
        {
            if (recipe == null) return;
            await Shell.Current.GoToAsync($"recipedetail?id={recipe.Id}");
        });

        BrowseRecipesCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("//HomePage");
        });

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        // Підвантажуємо кружечки інгредієнтів з API
        _ = LoadIngredientsAsync();
    }

    // ── Завантаження інгредієнтів з бекенду ───────────────────────────────
    private async Task LoadIngredientsAsync()
    {
        if (_recipeApiService == null) return;

        try
        {
            var ingredients = await _recipeApiService.GetAllIngredientsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Ingredients.Clear();
                foreach (var item in ingredients) Ingredients.Add(item);
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FavoritesViewModel.LoadIngredientsAsync error: {ex.Message}");
        }
    }

    // ── Sample-дані (поки нема реального API улюблених) ───────────────────
    private void LoadSampleFavorites()
    {
        var sampleRecipes = new List<RecipeModel>
        {
            new RecipeModel
            {
                Id = 2,
                Title = "Omelette",
                Description = "Fluffy French-style omelette with herbs, cheese and cherry tomatoes.",
                CookTimeMinutes = 10,
                Rating = 4.5,
                Category = "Breakfast",
                Difficulty = "Easy",
                Servings = 1,
                IsFavorite = true,
                ImageUrl = "https://images.pexels.com/photos/6294248/pexels-photo-6294248.jpeg?w=600"
            },
            new RecipeModel
            {
                Id = 4,
                Title = "Greek Salad",
                Description = "Fresh tomatoes, cucumbers, olives and feta with light olive oil dressing.",
                CookTimeMinutes = 5,
                Rating = 4.6,
                Category = "Salad",
                Difficulty = "Easy",
                Servings = 2,
                IsFavorite = true,
                ImageUrl = "https://images.pexels.com/photos/1059905/pexels-photo-1059905.jpeg?w=600"
            }
        };

        foreach (var recipe in sampleRecipes)
        {
            recipe.AccentColor = GetAccentColor(recipe.Category);
            recipe.AccentTextColor = GetAccentTextColor(recipe.Category);
            _allFavorites.Add(recipe);
        }
    }

    private string GetAccentColor(string category) => category switch
    {
        "Breakfast" => "#B45309",
        "Soup" => "#9F1239",
        "Pasta" => "#92400E",
        "Salad" => "#14532D",
        "Meat" => "#7C2D12",
        "Dessert" => "#86198F",
        _ => "#1E40AF"
    };

    private string GetAccentTextColor(string category) => category switch
    {
        "Breakfast" => "#92400E",
        "Soup" => "#881337",
        "Pasta" => "#78350F",
        "Salad" => "#166534",
        "Meat" => "#6B2110",
        "Dessert" => "#701A75",
        _ => "#1E3A8A"
    };

    // ── Тогл інгредієнта (виділити/зняти) ─────────────────────────────────
    private async Task OnToggleIngredientAsync(IngredientItem? ingredient)
    {
        if (ingredient == null) return;

        ingredient.IsSelected = !ingredient.IsSelected;
        _selectedIngredientsCount = Ingredients.Count(i => i.IsSelected);

        OnPropertyChanged(nameof(ShowFilterBadge));
        OnPropertyChanged(nameof(FilterBadgeText));
        OnPropertyChanged(nameof(HasActiveFilters));
        OnPropertyChanged(nameof(RecipesSectionTitle));

        await ApplyFiltersAsync();
    }

    // ── Основна логіка фільтрації ─────────────────────────────────────────
    private async Task ApplyFiltersAsync()
    {
        IEnumerable<RecipeModel> filtered = _allFavorites;

        // 1) Фільтр за вибраними інгредієнтами — через серверний пошук
        var selectedIds = Ingredients.Where(i => i.IsSelected).Select(i => i.Id).ToList();

        if (selectedIds.Any() && _recipeApiService != null)
        {
            try
            {
                // Запитуємо у бекенду рецепти з цими інгредієнтами,
                // потім перетинаємо з улюбленими (бо API повертає всі рецепти, не лише улюблені)
                var matching = await _recipeApiService.SearchAsync(selectedIds, null);
                var matchingIds = matching.Select(r => r.Id).ToHashSet();
                filtered = filtered.Where(r => matchingIds.Contains(r.Id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FavoritesViewModel.ApplyFiltersAsync (ingredients) error: {ex.Message}");
            }
        }

        // 2) Текстовий пошук — локально
        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            var q = _searchQuery.ToLower();
            filtered = filtered.Where(r =>
                r.Title.ToLower().Contains(q) ||
                r.Category.ToLower().Contains(q) ||
                r.Description.ToLower().Contains(q));
        }

        FilteredFavorites = new ObservableCollection<RecipeModel>(filtered);

        OnPropertyChanged(nameof(HasActiveFilters));
        OnPropertyChanged(nameof(RecipesSectionTitle));
        OnPropertyChanged(nameof(IsResultEmpty));
    }

    // ── Очистити всі фільтри ──────────────────────────────────────────────
    private async Task ClearAllFiltersAsync()
    {
        foreach (var ing in Ingredients) ing.IsSelected = false;
        _selectedIngredientsCount = 0;
        _searchQuery = string.Empty;

        OnPropertyChanged(nameof(SearchQuery));
        OnPropertyChanged(nameof(HasSearchQuery));
        OnPropertyChanged(nameof(ShowFilterBadge));
        OnPropertyChanged(nameof(FilterBadgeText));
        OnPropertyChanged(nameof(HasActiveFilters));
        OnPropertyChanged(nameof(RecipesSectionTitle));

        await ApplyFiltersAsync();
    }

    // ── Видалити з обраного ───────────────────────────────────────────────
    private async Task RemoveFavoriteAsync(RecipeModel? recipe)
    {
        if (recipe == null) return;

        recipe.IsFavorite = false;
        _allFavorites.Remove(recipe);

        await ApplyFiltersAsync();

        OnPropertyChanged(nameof(FavoritesCountText));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(IsNotEmpty));
        OnPropertyChanged(nameof(IsResultEmpty));
    }

    // ── Додати до обраного (викликається ззовні, з HomePage) ──────────────
    public void AddToFavorites(RecipeModel recipe)
    {
        if (!_allFavorites.Any(r => r.Id == recipe.Id))
        {
            recipe.IsFavorite = true;
            recipe.AccentColor = GetAccentColor(recipe.Category);
            recipe.AccentTextColor = GetAccentTextColor(recipe.Category);

            _allFavorites.Add(recipe);
            _ = ApplyFiltersAsync();

            OnPropertyChanged(nameof(FavoritesCountText));
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(IsNotEmpty));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}