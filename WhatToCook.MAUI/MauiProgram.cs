using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WhatToCook.MAUI.Services;
using WhatToCook.MAUI.Services.Interfaces;
using WhatToCook.MAUI.ViewModels;
using WhatToCook.MAUI.Views;

namespace WhatToCook.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // ── ВИПРАВЛЕНО: один ланцюжок викликів, без дублювання UseMauiApp ──
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
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

        // ── API-сервіси ───────────────────────────────────────────────────
        builder.Services.AddSingleton<IRecipeApiService, RecipeApiService>();
        builder.Services.AddSingleton<IFavoriteApiService, FavoriteApiService>();

        // ── Центральне сховище улюблених (Singleton — один на весь застосунок) ──
        builder.Services.AddSingleton<FavoritesStore>();

        // ── AppShell ──────────────────────────────────────────────────────
        builder.Services.AddSingleton<AppShell>();

        // ── Tab bar сторінки ──────────────────────────────────────────────
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<CalendarPage>();

        builder.Services.AddTransient<AddRecipeViewModel>();
        builder.Services.AddTransient<AddRecipePage>();

        builder.Services.AddTransient<FavoritesViewModel>();
        builder.Services.AddTransient<FavoritesPage>();

        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();

        // ── Детальна сторінка (навігаційна) ──────────────────────────────
        builder.Services.AddTransient<RecipeDetailViewModel>();
        builder.Services.AddTransient<RecipeDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}