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

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService _recipeApiService;
    private readonly FavoritesStore _favoritesStore;

    // ── Рецепти (результат поточного запиту) ──────────────────────────────
    public ObservableCollection<RecipeModel> Recipes { get; } = new();

    // ── Інгредієнти-чіпси ─────────────────────────────────────────────────
    public ObservableCollection<IngredientItem> Ingredients { get; } = new();

    // ── Пошуковий рядок ───────────────────────────────────────────────────
    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSearchQuery));
            _ = ApplySearchAsync();
        }
    }
    public bool HasSearchQuery => !string.IsNullOrEmpty(_searchQuery);

    // ── Стани UI ──────────────────────────────────────────────────────────
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    private bool _isEmpty;
    public bool IsEmpty
    {
        get => _isEmpty;
        set { _isEmpty = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotEmpty)); }
    }
    public bool IsNotEmpty => !_isEmpty;

    private bool _hasActiveFilters;
    public bool HasActiveFilters
    {
        get => _hasActiveFilters;
        set { _hasActiveFilters = value; OnPropertyChanged(); }
    }

    public string RecipesSectionTitle => HasActiveFilters
        ? "Search results"
        : "Popular recipes";

    private int _selectedCount;
    public bool ShowFilterBadge => _selectedCount > 0;
    public string FilterBadgeText => _selectedCount == 1
        ? "1 ingredient selected"
        : $"{_selectedCount} ingredients selected";

    // ── Команди ───────────────────────────────────────────────────────────
    public ICommand SearchCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand ToggleIngredientCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand ViewRecipeCommand { get; }

    public HomeViewModel(IRecipeApiService recipeApiService, FavoritesStore favoritesStore)
    {
        _recipeApiService = recipeApiService;
        _favoritesStore = favoritesStore;

        // Підписка на зміну у Store — щоб серце оновилось, коли користувач
        // видалив рецепт з FavoritesPage (треба синхронізувати з рецептами на HomePage).
        _favoritesStore.FavoritesChanged += OnFavoritesStoreChanged;

        SearchCommand = new Command(async () => await ApplySearchAsync());
        ClearSearchCommand = new Command(() => SearchQuery = string.Empty);
        ClearFiltersCommand = new Command(async () => await ClearFiltersAsync());

        ToggleIngredientCommand = new Command<IngredientItem>(
            async i => await OnToggleIngredientAsync(i));

        // ── ВИПРАВЛЕНО: тепер через FavoritesStore і IFavoriteApiService ──
        ToggleFavoriteCommand = new Command<RecipeModel>(async recipe =>
        {
            if (recipe == null) return;
            await _favoritesStore.ToggleAsync(recipe);
        });

        ViewRecipeCommand = new Command<RecipeModel>(async recipe =>
        {
            if (recipe == null) return;
            await Shell.Current.GoToAsync($"recipedetail?id={recipe.Id}");
        });

        _ = InitAsync();
    }

    /// <summary>
    /// Викликається з HomePage.OnAppearing — щоб новий рецепт,
    /// створений на AddRecipePage, одразу з'явився у списку.
    /// </summary>
    public async Task ReloadAsync()
    {
        await ApplySearchAsync();
    }

    private async Task InitAsync()
    {
        try
        {
            IsBusy = true;

            // Підтягуємо актуальний список улюблених, щоб серця були заповнені при старті
            await _favoritesStore.EnsureInitializedAsync();

            var ingredientsTask = _recipeApiService.GetAllIngredientsAsync();
            var recipesTask = _recipeApiService.GetAllAsync();

            await Task.WhenAll(ingredientsTask, recipesTask);

            var recipes = (await recipesTask).ToList();
            _favoritesStore.ApplyFavoriteFlags(recipes);

            Ingredients.Clear();
            foreach (var ing in await ingredientsTask) Ingredients.Add(ing);

            Recipes.Clear();
            foreach (var r in recipes) Recipes.Add(r);

            IsEmpty = !Recipes.Any();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"HomeViewModel.InitAsync error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnToggleIngredientAsync(IngredientItem? ingredient)
    {
        if (ingredient == null) return;

        ingredient.IsSelected = !ingredient.IsSelected;

        _selectedCount = Ingredients.Count(i => i.IsSelected);
        OnPropertyChanged(nameof(FilterBadgeText));
        OnPropertyChanged(nameof(ShowFilterBadge));

        await ApplySearchAsync();
    }

    private async Task ApplySearchAsync()
    {
        var selectedIds = Ingredients
            .Where(i => i.IsSelected)
            .Select(i => i.Id)
            .ToList();

        HasActiveFilters = selectedIds.Any() || !string.IsNullOrWhiteSpace(_searchQuery);
        OnPropertyChanged(nameof(RecipesSectionTitle));

        try
        {
            IsBusy = true;

            IEnumerable<RecipeModel> results;
            if (!selectedIds.Any() && string.IsNullOrWhiteSpace(_searchQuery))
                results = await _recipeApiService.GetAllAsync();
            else
                results = await _recipeApiService.SearchAsync(selectedIds, _searchQuery);

            var list = results.ToList();
            _favoritesStore.ApplyFavoriteFlags(list);

            Recipes.Clear();
            foreach (var r in list) Recipes.Add(r);

            IsEmpty = !Recipes.Any();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"HomeViewModel.ApplySearchAsync error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ClearFiltersAsync()
    {
        foreach (var ing in Ingredients) ing.IsSelected = false;
        _searchQuery = string.Empty;
        _selectedCount = 0;
        HasActiveFilters = false;

        OnPropertyChanged(nameof(SearchQuery));
        OnPropertyChanged(nameof(HasSearchQuery));
        OnPropertyChanged(nameof(FilterBadgeText));
        OnPropertyChanged(nameof(ShowFilterBadge));
        OnPropertyChanged(nameof(RecipesSectionTitle));

        try
        {
            IsBusy = true;
            var all = (await _recipeApiService.GetAllAsync()).ToList();
            _favoritesStore.ApplyFavoriteFlags(all);

            Recipes.Clear();
            foreach (var r in all) Recipes.Add(r);
            IsEmpty = !Recipes.Any();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"HomeViewModel.ClearFiltersAsync error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnFavoritesStoreChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _favoritesStore.ApplyFavoriteFlags(Recipes);
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}