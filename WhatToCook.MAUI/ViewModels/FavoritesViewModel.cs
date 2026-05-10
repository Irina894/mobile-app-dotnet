using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;

using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels;

public class FavoritesViewModel : INotifyPropertyChanged
{
    private ObservableCollection<RecipeModel> _allFavorites;
    private ObservableCollection<RecipeModel> _filteredFavorites;
    private string _searchQuery = string.Empty;

    public ObservableCollection<RecipeModel> FilteredFavorites
    {
        get => _filteredFavorites;
        set { _filteredFavorites = value; OnPropertyChanged(); }
    }

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

    public string FavoritesCountText => _allFavorites.Count == 0
        ? "No favorites yet"
        : $"{_allFavorites.Count} favorite recipe{(_allFavorites.Count == 1 ? "" : "s")}";

    public bool IsEmpty => _allFavorites.Count == 0;
    public bool IsNotEmpty => _allFavorites.Count > 0;

    public ICommand SearchCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand RemoveFavoriteCommand { get; }
    public ICommand ViewRecipeCommand { get; }
    public ICommand BrowseRecipesCommand { get; }
    public ICommand BackCommand { get; }

    public FavoritesViewModel()
    {
        _allFavorites = new ObservableCollection<RecipeModel>();

        // Тимчасові дані — пізніше замінимо на завантаження з API
        LoadSampleFavorites();

        _filteredFavorites = new ObservableCollection<RecipeModel>(_allFavorites);

        SearchCommand = new Command(ApplySearch);

        ClearSearchCommand = new Command(() => SearchQuery = string.Empty);

        RemoveFavoriteCommand = new Command<RecipeModel>(RemoveFavorite);

        ViewRecipeCommand = new Command<RecipeModel>(recipe =>
        {
            // TODO: Shell.Current.GoToAsync($"recipedetail?id={recipe.Id}");
        });

        BrowseRecipesCommand = new Command(() =>
        {
            // TODO: Shell.Current.GoToAsync("//HomePage");
        });

        BackCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }

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

    private void ApplySearch()
    {
        if (string.IsNullOrWhiteSpace(_searchQuery))
        {
            FilteredFavorites = new ObservableCollection<RecipeModel>(_allFavorites);
            return;
        }

        var q = _searchQuery.ToLower();
        var filtered = _allFavorites.Where(r =>
            r.Title.ToLower().Contains(q) ||
            r.Category.ToLower().Contains(q) ||
            r.Description.ToLower().Contains(q)
        );

        FilteredFavorites = new ObservableCollection<RecipeModel>(filtered);
    }

    private void RemoveFavorite(RecipeModel? recipe)
    {
        if (recipe == null) return;

        recipe.IsFavorite = false;
        _allFavorites.Remove(recipe);

        ApplySearch();

        OnPropertyChanged(nameof(FavoritesCountText));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(IsNotEmpty));
    }

    public void AddToFavorites(RecipeModel recipe)
    {
        if (!_allFavorites.Any(r => r.Id == recipe.Id))
        {
            recipe.IsFavorite = true;
            recipe.AccentColor = GetAccentColor(recipe.Category);
            recipe.AccentTextColor = GetAccentTextColor(recipe.Category);

            _allFavorites.Add(recipe);
            ApplySearch();

            OnPropertyChanged(nameof(FavoritesCountText));
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(IsNotEmpty));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}