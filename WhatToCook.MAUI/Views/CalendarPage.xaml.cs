using WhatToCook.MAUI.ViewModels;

namespace WhatToCook.MAUI.Views;

public partial class CalendarPage : ContentPage
{
    public CalendarPage(CalendarViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}