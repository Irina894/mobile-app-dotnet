using System.Collections.ObjectModel;
using System.Windows.Input;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services;

namespace WhatToCook.MAUI.ViewModels
{
    public class RecipeListViewModel : BaseViewModel
    {
        private readonly IRecipeApiService _recipeApiService;

        public ObservableCollection<RecipeModel> Recipes { get; set; }

        public ICommand LoadRecipesCommand { get; }

        public RecipeListViewModel(IRecipeApiService recipeApiService)
        {
            _recipeApiService = recipeApiService;

            Title = "Recipes";
            Recipes = new ObservableCollection<RecipeModel>();

            LoadRecipesCommand = new Command(async () => await LoadRecipesAsync());
        }

        private async Task LoadRecipesAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Recipes.Clear();

                var recipes = await _recipeApiService.GetAllAsync();

                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}