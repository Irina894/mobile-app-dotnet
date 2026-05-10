using Microsoft.Extensions.Logging;
using WhatToCook.MAUI.Services;
using WhatToCook.MAUI.Services.Interfaces;
using WhatToCook.MAUI.ViewModels;
using WhatToCook.MAUI.ViewModels.Recipes;
using WhatToCook.MAUI.Views;
using WhatToCook.MAUI.Views.Recipe;

namespace WhatToCook.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── HttpClient ────────────────────────────────────────────────────
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://5q419q9m-7086.euw.devtunnels.ms/")
        });

        // ── Сервіси ───────────────────────────────────────────────────────
        builder.Services.AddSingleton<IRecipeApiService, RecipeApiService>();

        // ── AppShell ──────────────────────────────────────────────────────
        builder.Services.AddSingleton<AppShell>();

        // ── Сторінки та ViewModel (Tab bar) ──────────────────────────────
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<RecipeListViewModel>();
        builder.Services.AddTransient<RecipeListPage>();

        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();

        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<CalendarPage>();

        builder.Services.AddTransient<AddRecipeViewModel>();
        builder.Services.AddTransient<AddRecipePage>();

        builder.Services.AddTransient<FavoritesViewModel>();
        builder.Services.AddTransient<FavoritesPage>();

        // ── Детальна сторінка рецепта (навігаційна, не в Tab bar) ────────
        builder.Services.AddTransient<RecipeDetailViewModel>();
        builder.Services.AddTransient<RecipeDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}