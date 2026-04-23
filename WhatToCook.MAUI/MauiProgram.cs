using Microsoft.Extensions.Logging;
using WhatToCook.MAUI.Services;
using WhatToCook.MAUI.ViewModels;
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

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5033/")
            });

            builder.Services.AddSingleton<IRecipeApiService, RecipeApiService>();

            builder.Services.AddTransient<RecipeListViewModel>();
            builder.Services.AddTransient<RecipeListPage>();
      
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}