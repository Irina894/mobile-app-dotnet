using System.Diagnostics;
using System.Net.Http.Json;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;

namespace WhatToCook.MAUI.Services;

public class FavoriteApiService : IFavoriteApiService
{
    private readonly HttpClient _httpClient;

    public FavoriteApiService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<bool> ToggleAsync(int userId, int recipeId)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"api/FavoriteRecipe/toggle?userId={userId}&recipeId={recipeId}", null);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Toggle failed: {response.StatusCode}");
                return false;
            }
            var payload = await response.Content.ReadFromJsonAsync<ToggleResult>();
            return payload?.isFavorite ?? false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FavoriteApiService.ToggleAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<Recipe>> GetUserFavoriteRecipesAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/FavoriteRecipe/user/{userId}/recipes");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Recipe>>() ?? new();
            return new List<Recipe>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FavoriteApiService.GetUserFavoriteRecipesAsync error: {ex.Message}");
            return new List<Recipe>();
        }
    }

    private class ToggleResult { public bool isFavorite { get; set; } }
}