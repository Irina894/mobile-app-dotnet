using WhatToCook.MAUI.ViewModels;

namespace WhatToCook.MAUI.Views
{
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
            {
                _viewModel.LoadRecipesCommand.Execute(null);
            }
        }
    }
}