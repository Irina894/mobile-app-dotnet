namespace WhatToCook.MAUI.Views;

using WhatToCook.MAUI.ViewModels;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesViewModel _vm;
    public FavoritesPage(FavoritesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }
   
}