using Microsoft.Maui.Storage;
using WhatToCook.MAUI.ViewModels;

namespace WhatToCook.MAUI.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}