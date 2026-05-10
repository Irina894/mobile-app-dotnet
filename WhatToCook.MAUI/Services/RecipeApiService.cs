using System.Net.Http.Json;
using System.Diagnostics;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;
using WhatToCook.MAUI.ViewModels; // для RecipeDetailModel

namespace WhatToCook.MAUI.Services;

public class RecipeApiService : IRecipeApiService
{
    private readonly HttpClient _httpClient;

    public RecipeApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ── Всі рецепти ───────────────────────────────────────────────────────
    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/recipes");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Recipe>>() ?? new();
            Debug.WriteLine($"API Error GetAll: {response.StatusCode}");
            return new List<Recipe>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception GetAllAsync: {ex.Message}");
            return new List<Recipe>();
        }
    }

    // ── Один рецепт (базова модель, без інгредієнтів) ─────────────────────
    public async Task<Recipe?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Recipe>($"api/recipes/{id}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception GetByIdAsync: {ex.Message}");
            return null;
        }
    }

    // ── Один рецепт З інгредієнтами (для деталей) ─────────────────────────
    // Той самий endpoint api/recipes/{id} — але десеріалізуємо в RecipeDetailModel
    // що має поле Ingredients[]
    public async Task<RecipeDetailModel?> GetByIdWithIngredientsAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/recipes/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RecipeDetailModel>();

            Debug.WriteLine($"API Error GetByIdWithIngredients: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception GetByIdWithIngredientsAsync: {ex.Message}");
            return null;
        }
    }

    // ── Створення (без інгредієнтів, для сумісності) ──────────────────────
    public async Task<Recipe?> CreateAsync(Recipe recipe)
    {
        return await CreateWithIngredientsAsync(new
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
            isMyRecipe = recipe.IsMyRecipe,
            ingredientIds = new List<int>()
        });
    }

    // ── Створення З інгредієнтами ─────────────────────────────────────────
    public async Task<Recipe?> CreateWithIngredientsAsync(object payload)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/recipes", payload);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Recipe>();

            var error = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Create Error: {response.StatusCode} — {error}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception CreateWithIngredientsAsync: {ex.Message}");
            return null;
        }
    }

    // ── Оновлення ─────────────────────────────────────────────────────────
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
            Debug.WriteLine($"Exception UpdateAsync: {ex.Message}");
            return false;
        }
    }

    // ── Видалення ─────────────────────────────────────────────────────────
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/recipes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception DeleteAsync: {ex.Message}");
            return false;
        }
    }

    // ── Всі інгредієнти ───────────────────────────────────────────────────
    public async Task<IEnumerable<IngredientItem>> GetAllIngredientsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/ingredients");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<IngredientItem>>() ?? new();
            return new List<IngredientItem>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception GetAllIngredientsAsync: {ex.Message}");
            return new List<IngredientItem>();
        }
    }
}