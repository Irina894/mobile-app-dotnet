using WhatToCook.MAUI.ViewModels;

namespace WhatToCook.MAUI.Views;

public partial class AddRecipePage : ContentPage
{
    // Сторінка тепер приймає свою ViewModel через конструктор
    public AddRecipePage(AddRecipeViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}