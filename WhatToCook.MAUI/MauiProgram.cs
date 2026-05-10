using Microsoft.Extensions.Logging;
using WhatToCook.MAUI.Services;
using WhatToCook.MAUI.Services.Interfaces;
using WhatToCook.MAUI.ViewModels;
using WhatToCook.MAUI.ViewModels.Recipes;
using WhatToCook.MAUI.Views;

namespace WhatToCook.MAUI
{
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

            // Налаштування HttpClient для Android-емулятора (10.0.2.2) 
            // або реального пристрою (треба буде змінити на IP твого ПК пізніше)
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://5q419q9m-7086.euw.devtunnels.ms/")
            });

            // Реєстрація сервісів
            builder.Services.AddSingleton<IRecipeApiService, RecipeApiService>();

            // КРИТИЧНО: Реєстрація AppShell
            builder.Services.AddSingleton<AppShell>();

            // Реєстрація ViewModel та Сторінок
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}