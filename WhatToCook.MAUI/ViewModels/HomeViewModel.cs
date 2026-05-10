using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;

// Alias вирішує конфлікт: де б не було namespace "Recipe" — ми явно кажемо що це клас з Models
using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly IRecipeApiService _recipeApiService;
    private ObservableCollection<RecipeModel> _allRecipes = new();
    private ObservableCollection<RecipeModel> _filteredRecipes = new();
    private string _searchQuery = string.Empty;

    public ObservableCollection<RecipeModel> FilteredRecipes
    {
        get => _filteredRecipes;
        set { _filteredRecipes = value; OnPropertyChanged(); }
    }

    public ObservableCollection<IngredientItem> Ingredients { get; set; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSearchQuery));
            ApplySearch();
        }
    }

    public bool HasSearchQuery => !string.IsNullOrEmpty(_searchQuery);

    public ICommand SearchCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand ViewRecipeCommand { get; }
    public ICommand SeeAllIngredientsCommand { get; }
    public ICommand SelectIngredientCommand { get; }

    public HomeViewModel(IRecipeApiService recipeApiService)
    {
        _recipeApiService = recipeApiService;

        SearchCommand = new Command(ApplySearch);
        ClearSearchCommand = new Command(() => SearchQuery = string.Empty);

        ToggleFavoriteCommand = new Command<RecipeModel>(async (recipe) =>
        {
            recipe.IsFavorite = !recipe.IsFavorite;
            await _recipeApiService.UpdateAsync(recipe);
        });

        ViewRecipeCommand = new Command<RecipeModel>(recipe => { /* Навігація */ });
        SeeAllIngredientsCommand = new Command(() => { /* Навігація */ });
        SelectIngredientCommand = new Command<IngredientItem>(ingredient => SearchQuery = ingredient.Name);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var recipes = await _recipeApiService.GetAllAsync();
            _allRecipes = new ObservableCollection<RecipeModel>(recipes);

            var ingredients = await _recipeApiService.GetAllIngredientsAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Ingredients.Clear();
                foreach (var item in ingredients)
                {
                    Ingredients.Add(item);
                }
                ApplySearch();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    private void ApplySearch()
    {
        if (string.IsNullOrWhiteSpace(_searchQuery))
        {
            FilteredRecipes = new ObservableCollection<RecipeModel>(_allRecipes);
            return;
        }

        var q = _searchQuery.ToLower();
        var filtered = _allRecipes.Where(r =>
            r.Title.ToLower().Contains(q) ||
            r.Category.ToLower().Contains(q) ||
            r.Description.ToLower().Contains(q)
        );

        FilteredRecipes = new ObservableCollection<RecipeModel>(filtered);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}