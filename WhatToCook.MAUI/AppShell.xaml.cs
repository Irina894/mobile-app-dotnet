namespace WhatToCook.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("recipedetail", typeof(Views.RecipeDetailPage));
    }
}