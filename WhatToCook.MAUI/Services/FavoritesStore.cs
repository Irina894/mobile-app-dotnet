using System.Collections.ObjectModel;
using System.Diagnostics;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;

namespace WhatToCook.MAUI.Services;
public class FavoritesStore
{
    private readonly IFavoriteApiService _favoriteApi;
    private const int CurrentUserId = 1;

    // Повний список улюблених рецептів (для FavoritesPage)
    public ObservableCollection<Recipe> Favorites { get; } = new();

    // Швидкий індекс ID → щоб відразу знати, чи рецепт улюблений
    private readonly HashSet<int> _favoriteIds = new();

    // Прапорець, що уникає повторного завантаження при кожному заході
    private bool _isInitialized;

    public event EventHandler? FavoritesChanged;

    public FavoritesStore(IFavoriteApiService favoriteApi)
    {
        _favoriteApi = favoriteApi;
    }

    /// <summary>
    /// Чи входить рецепт у список улюблених?
    /// </summary>
    public bool IsFavorite(int recipeId) => _favoriteIds.Contains(recipeId);

    /// <summary>
    /// Завантажує улюблені з бекенду. Викликається з FavoritesPage.OnAppearing
    /// і одразу при старті застосунку (через EnsureInitializedAsync).
    /// </summary>
    public async Task LoadAsync()
    {
        try
        {
            var fromApi = await _favoriteApi.GetUserFavoriteRecipesAsync(CurrentUserId);
            var list = fromApi.ToList();

            // Оновлюємо колекції на UI-потоці
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Favorites.Clear();
                _favoriteIds.Clear();
                foreach (var r in list)
                {
                    r.IsFavorite = true; // на FavoritesPage завжди true
                    Favorites.Add(r);
                    _favoriteIds.Add(r.Id);
                }
                _isInitialized = true;
                FavoritesChanged?.Invoke(this, EventArgs.Empty);
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FavoritesStore.LoadAsync error: {ex.Message}");
        }
    }

    /// <summary>
    /// Викликається з HomeViewModel при першому завантаженні,
    /// щоб ми знали які рецепти позначити серцем, перш ніж користувач
    /// зайде у Favorites.
    /// </summary>
    public async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;
        await LoadAsync();
    }

    /// <summary>
    /// Перемикає статус улюбленого:
    /// 1) миттєво змінює UI (recipe.IsFavorite + колекція Favorites);
    /// 2) шле зміну на бекенд у фоні;
    /// 3) якщо бекенд відповів інакше (помилка, конфлікт) — відкочуємо.
    /// </summary>
    public async Task ToggleAsync(Recipe recipe)
    {
        if (recipe == null) return;

        var wasFavorite = _favoriteIds.Contains(recipe.Id);

        // ── OPTIMISTIC UPDATE: миттєвий відгук для UI ─────────────────────
        if (wasFavorite)
        {
            _favoriteIds.Remove(recipe.Id);
            var existing = Favorites.FirstOrDefault(r => r.Id == recipe.Id);
            if (existing != null) Favorites.Remove(existing);
            recipe.IsFavorite = false;
        }
        else
        {
            _favoriteIds.Add(recipe.Id);
            // Якщо такого рецепту немає у Favorites (а він з HomePage) — додаємо.
            if (!Favorites.Any(r => r.Id == recipe.Id))
                Favorites.Add(recipe);
            recipe.IsFavorite = true;
        }

        FavoritesChanged?.Invoke(this, EventArgs.Empty);

        // ── SYNC З БЕКЕНДОМ ──────────────────────────────────────────────
        try
        {
            var serverState = await _favoriteApi.ToggleAsync(CurrentUserId, recipe.Id);

            // Якщо сервер каже інше — відкочуємо до того стану що на сервері
            if (serverState != recipe.IsFavorite)
            {
                if (serverState)
                {
                    _favoriteIds.Add(recipe.Id);
                    if (!Favorites.Any(r => r.Id == recipe.Id))
                        Favorites.Add(recipe);
                }
                else
                {
                    _favoriteIds.Remove(recipe.Id);
                    var existing = Favorites.FirstOrDefault(r => r.Id == recipe.Id);
                    if (existing != null) Favorites.Remove(existing);
                }
                recipe.IsFavorite = serverState;
                FavoritesChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FavoritesStore.ToggleAsync (server) error: {ex.Message}");
            // На випадок offline — лишаємо optimistic-стан, синхронізується при наступному LoadAsync.
        }
    }
    public void ApplyFavoriteFlags(IEnumerable<Recipe> recipes)
    {
        foreach (var r in recipes)
            r.IsFavorite = _favoriteIds.Contains(r.Id);
    }
}