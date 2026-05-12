using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;
using WhatToCook.MAUI.ViewModels.Recipe;
using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels.Recipes;

public class RecipeListViewModel : BaseViewModel
{
    private readonly IRecipeApiService _recipeApiService;

    public ObservableCollection<RecipeModel> Recipes { get; set; } = new();

    public ICommand LoadRecipesCommand { get; }

    public RecipeListViewModel(IRecipeApiService recipeApiService)
    {
        _recipeApiService = recipeApiService;
        Title = "Recipes";

        LoadRecipesCommand = new Command(async () => await LoadRecipesAsync());
    }

    public async Task LoadRecipesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var recipesFromDb = await _recipeApiService.GetAllAsync();
            var recipeList = recipesFromDb.ToList();

            Recipes.Clear();

            foreach (var recipe in recipeList)
            {
                Recipes.Add(recipe);
            }

            Debug.WriteLine($"Loaded {recipeList.Count} recipes from API.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading recipes: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Could not load recipes from server.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}