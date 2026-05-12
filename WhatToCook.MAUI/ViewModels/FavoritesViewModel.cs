using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services;
using WhatToCook.MAUI.Services.Interfaces;

using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels;

public class FavoritesViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService _recipeApiService;
    private readonly FavoritesStore _favoritesStore;

    private ObservableCollection<RecipeModel> _filteredFavorites = new();
    private string _searchQuery = string.Empty;

    public ObservableCollection<RecipeModel> FilteredFavorites
    {
        get => _filteredFavorites;
        set { _filteredFavorites = value; OnPropertyChanged(); }
    }

    public ObservableCollection<IngredientItem> Ingredients { get; } = new();

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

    public string FavoritesCountText => _favoritesStore.Favorites.Count == 0
        ? "No favorites yet"
        : $"{_favoritesStore.Favorites.Count} favorite recipe{(_favoritesStore.Favorites.Count == 1 ? "" : "s")}";

    public string RecipesSectionTitle => HasActiveFilters
        ? "Search results"
        : "All favorites";

    public bool IsEmpty => _favoritesStore.Favorites.Count == 0;
    public bool IsNotEmpty => _favoritesStore.Favorites.Count > 0;
    public bool IsResultEmpty =>
        _favoritesStore.Favorites.Count > 0 && _filteredFavorites.Count == 0;

    // ── Команди ───────────────────────────────────────────────────────────
    public ICommand SearchCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand ToggleIngredientCommand { get; }
    public ICommand RemoveFavoriteCommand { get; }
    public ICommand ViewRecipeCommand { get; }
    public ICommand BrowseRecipesCommand { get; }
    public ICommand BackCommand { get; }

    public FavoritesViewModel(IRecipeApiService recipeApiService, FavoritesStore favoritesStore)
    {
        _recipeApiService = recipeApiService;
        _favoritesStore = favoritesStore;

        // Реагуємо на зміни Store з будь-якого місця (HomePage, Detail)
        _favoritesStore.FavoritesChanged += OnStoreChanged;

        SearchCommand = new Command(async () => await ApplyFiltersAsync());
        ClearSearchCommand = new Command(() => SearchQuery = string.Empty);
        ClearFiltersCommand = new Command(async () => await ClearAllFiltersAsync());
        ToggleIngredientCommand = new Command<IngredientItem>(async i => await OnToggleIngredientAsync(i));

        // ── ВИДАЛЕННЯ З УЛЮБЛЕНИХ ────────────────────────────────────────
        // Натискання серця → Store.ToggleAsync → бекенд + миттєвий UI update.
        RemoveFavoriteCommand = new Command<RecipeModel>(async recipe =>
        {
            if (recipe == null) return;
            await _favoritesStore.ToggleAsync(recipe);
            // OnStoreChanged спрацює і викличе RefreshList
        });

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

        // Перше відображення (Store може ще бути порожнім — ок,
        // OnAppearing → Reload потім підтягне з API)
        RefreshList();

        // Підвантажуємо інгредієнти для чіпсів-фільтрів
        _ = LoadIngredientsAsync();
    }

    /// <summary>
    /// Викликається з FavoritesPage.OnAppearing.
    /// Завантажує улюблені з бекенду через Store (бо це джерело істини),
    /// потім оновлює список на сторінці.
    /// </summary>
    public async Task ReloadAsync()
    {
        await _favoritesStore.LoadAsync();
        // RefreshList викличеться через подію FavoritesChanged
    }

    private async Task LoadIngredientsAsync()
    {
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

    private void OnStoreChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(RefreshList);
    }

    private void RefreshList()
    {
        // Підфарбовуємо рецепти для гарного UI (колір залежить від категорії)
        foreach (var r in _favoritesStore.Favorites)
        {
            if (string.IsNullOrEmpty(r.AccentColor) || r.AccentColor == "#1E40AF")
                r.AccentColor = GetAccentColor(r.Category);
            if (string.IsNullOrEmpty(r.AccentTextColor) || r.AccentTextColor == "#1E3A8A")
                r.AccentTextColor = GetAccentTextColor(r.Category);
        }

        _ = ApplyFiltersAsync();

        OnPropertyChanged(nameof(FavoritesCountText));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(IsNotEmpty));
        OnPropertyChanged(nameof(IsResultEmpty));
    }

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

    private async Task ApplyFiltersAsync()
    {
        IEnumerable<RecipeModel> filtered = _favoritesStore.Favorites;

        // 1) Фільтр за інгредієнтами — серверний пошук + перетин з улюбленими
        var selectedIds = Ingredients.Where(i => i.IsSelected).Select(i => i.Id).ToList();
        if (selectedIds.Any())
        {
            try
            {
                var matching = await _recipeApiService.SearchAsync(selectedIds, null);
                var matchingIds = matching.Select(r => r.Id).ToHashSet();
                filtered = filtered.Where(r => matchingIds.Contains(r.Id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FavoritesViewModel.ApplyFiltersAsync (ingredients) error: {ex.Message}");
            }
        }

        // 2) Текстовий пошук — локально по назві/категорії/опису
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}