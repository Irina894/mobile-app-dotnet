using System.Net.Http.Json;
using System.Diagnostics;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;

namespace WhatToCook.MAUI.Services;

public class RecipeApiService : IRecipeApiService
{
    private readonly HttpClient _httpClient;

    public RecipeApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ── Отримання всіх рецептів ────────────────────────────────────────────
    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/recipes");

            if (response.IsSuccessStatusCode)
            {
                var recipes = await response.Content.ReadFromJsonAsync<List<Recipe>>();
                return recipes ?? new List<Recipe>();
            }

            var error = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"API Error GetAll: {response.StatusCode} — {error}");
            return new List<Recipe>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetAllAsync: {ex.Message}");
            return new List<Recipe>();
        }
    }

    // ── Отримання одного рецепта ───────────────────────────────────────────
    public async Task<Recipe?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Recipe>($"api/recipes/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetByIdAsync: {ex.Message}");
            return null;
        }
    }

    // ── Створення нового рецепта ───────────────────────────────────────────
    // Надсилаємо анонімний об'єкт що відповідає CreateRecipeDto на сервері
    // Це безпечніше ніж надсилати всю MAUI модель з [JsonIgnore] полями
    public async Task<Recipe?> CreateAsync(Recipe recipe)
    {
        try
        {
            // Формуємо об'єкт що точно відповідає CreateRecipeDto на API
            var payload = new
            {
                title = recipe.Title,
                description = recipe.Description,
                cookTimeMinutes = recipe.CookTimeMinutes,
                category = recipe.Category,
                difficulty = recipe.Difficulty,
                servings = recipe.Servings,
                imageUrl = recipe.ImageUrl,
                accentColor = recipe.AccentColor,
                accentTextColor = recipe.AccentTextColor,
                isMyRecipe = recipe.IsMyRecipe
            };

            var response = await _httpClient.PostAsJsonAsync("api/recipes", payload);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Recipe>();
            }

            var error = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Create API Error: {response.StatusCode} — {error}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in CreateAsync: {ex.Message}");
            return null;
        }
    }

    // ── Оновлення рецепта ──────────────────────────────────────────────────
    public async Task<bool> UpdateAsync(Recipe recipe)
    {
        try
        {
            var payload = new
            {
                title = recipe.Title,
                description = recipe.Description,
                cookTimeMinutes = recipe.CookTimeMinutes,
                category = recipe.Category,
                difficulty = recipe.Difficulty,
                servings = recipe.Servings,
                imageUrl = recipe.ImageUrl,
                accentColor = recipe.AccentColor,
                accentTextColor = recipe.AccentTextColor,
                isMyRecipe = recipe.IsMyRecipe
            };

            var response = await _httpClient.PutAsJsonAsync($"api/recipes/{recipe.Id}", payload);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in UpdateAsync: {ex.Message}");
            return false;
        }
    }

    // ── Видалення рецепта ──────────────────────────────────────────────────
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/recipes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in DeleteAsync: {ex.Message}");
            return false;
        }
    }

    // ── Отримання інгредієнтів ─────────────────────────────────────────────
    public async Task<IEnumerable<IngredientItem>> GetAllIngredientsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/ingredients");
            if (response.IsSuccessStatusCode)
            {
                var ingredients = await response.Content.ReadFromJsonAsync<List<IngredientItem>>();
                return ingredients ?? new List<IngredientItem>();
            }
            return new List<IngredientItem>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetAllIngredientsAsync: {ex.Message}");
            return new List<IngredientItem>();
        }
    }
}