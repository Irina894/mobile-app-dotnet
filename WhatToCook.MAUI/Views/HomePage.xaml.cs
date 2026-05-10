using WhatToCook.MAUI.ViewModels;

namespace WhatToCook.MAUI.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}