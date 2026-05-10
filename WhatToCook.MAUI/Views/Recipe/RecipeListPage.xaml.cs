using WhatToCook.MAUI.ViewModels.Recipes;

namespace WhatToCook.MAUI.Views.Recipe;  // ← namespace з папкою Recipe

public partial class RecipeListPage : ContentPage
{
    private readonly RecipeListViewModel _viewModel;

    public RecipeListPage(RecipeListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.LoadRecipesCommand.CanExecute(null))
            _viewModel.LoadRecipesCommand.Execute(null);
    }
}