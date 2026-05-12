using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;
using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService _recipeApiService;

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

    // Заголовок секції рецептів — змінюється залежно від того, чи є фільтри
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

    public HomeViewModel(IRecipeApiService recipeApiService)
    {
        _recipeApiService = recipeApiService;

        SearchCommand = new Command(async () => await ApplySearchAsync());
        ClearSearchCommand = new Command(() => SearchQuery = string.Empty);
        ClearFiltersCommand = new Command(async () => await ClearFiltersAsync());

        ToggleIngredientCommand = new Command<IngredientItem>(
            async i => await OnToggleIngredientAsync(i));

        ToggleFavoriteCommand = new Command<RecipeModel>(async recipe =>
        {
            if (recipe == null) return;
            recipe.IsFavorite = !recipe.IsFavorite;
            await _recipeApiService.UpdateAsync(recipe);
            // TODO: коли підключите FavoriteApiService — викликати тут Toggle
        });

        ViewRecipeCommand = new Command<RecipeModel>(async recipe =>
        {
            if (recipe == null) return;
            await Shell.Current.GoToAsync($"recipedetail?id={recipe.Id}");
        });

        _ = InitAsync();
    }

    // ── Ініціалізація ─────────────────────────────────────────────────────
    private async Task InitAsync()
    {
        try
        {
            IsBusy = true;

            var ingredientsTask = _recipeApiService.GetAllIngredientsAsync();
            var recipesTask = _recipeApiService.GetAllAsync();

            await Task.WhenAll(ingredientsTask, recipesTask);

            Ingredients.Clear();
            foreach (var ing in await ingredientsTask) Ingredients.Add(ing);

            Recipes.Clear();
            foreach (var r in await recipesTask) Recipes.Add(r);

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

    // ── Тогл чіпса інгредієнта ───────────────────────────────────────────
    private async Task OnToggleIngredientAsync(IngredientItem? ingredient)
    {
        if (ingredient == null) return;

        ingredient.IsSelected = !ingredient.IsSelected;

        _selectedCount = Ingredients.Count(i => i.IsSelected);
        OnPropertyChanged(nameof(FilterBadgeText));
        OnPropertyChanged(nameof(ShowFilterBadge));

        await ApplySearchAsync();
    }

    // ── Застосування пошуку (текст + інгредієнти, серверний) ─────────────
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

            Recipes.Clear();
            foreach (var r in results) Recipes.Add(r);

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

    // ── Очистити всі фільтри ─────────────────────────────────────────────
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
            var all = await _recipeApiService.GetAllAsync();
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}