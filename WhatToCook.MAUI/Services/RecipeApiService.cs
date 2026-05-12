using System.Net.Http.Json;
using System.Diagnostics;
using WhatToCook.MAUI.Models;
using WhatToCook.MAUI.Services.Interfaces;
using WhatToCook.MAUI.ViewModels;

namespace WhatToCook.MAUI.Services;

public class RecipeApiService : IRecipeApiService
{
    private readonly HttpClient _httpClient;
    public const int CurrentUserId = 1;
    public RecipeApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/recipes?userId={CurrentUserId}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Recipe>>() ?? new();
            Debug.WriteLine($"API Error GetAll: {response.StatusCode}");
            return new List<Recipe>();
        }
        catch (Exception ex) { Debug.WriteLine($"Exception GetAllAsync: {ex.Message}"); return new List<Recipe>(); }
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        try { return await _httpClient.GetFromJsonAsync<Recipe>($"api/recipes/{id}"); }
        catch (Exception ex) { Debug.WriteLine($"Exception GetByIdAsync: {ex.Message}"); return null; }
    }

    public async Task<RecipeDetailModel?> GetByIdWithIngredientsAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/recipes/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RecipeDetailModel>();
            return null;
        }
        catch (Exception ex) { Debug.WriteLine($"Exception GetByIdWithIngredients: {ex.Message}"); return null; }
    }

    // ── SEARCH ────────────────────────────────────────────────────────────
    // Будує URL: api/recipes/search?ids=1,5,7&q=soup
    public async Task<IEnumerable<Recipe>> SearchAsync(
        List<int> ingredientIds,
        string? query = null)
    {
        try
        {
            var url = "api/recipes/search";
            var queryParams = new List<string>();

            queryParams.Add($"userId={CurrentUserId}");

            if (ingredientIds.Any())
                queryParams.Add($"ids={string.Join(",", ingredientIds)}");

            if (!string.IsNullOrWhiteSpace(query))
                queryParams.Add($"q={Uri.EscapeDataString(query)}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Recipe>>() ?? new();

            Debug.WriteLine($"Search Error: {response.StatusCode}");
            return new List<Recipe>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception SearchAsync: {ex.Message}");
            return new List<Recipe>();
        }
    }

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
        catch (Exception ex) { Debug.WriteLine($"Exception CreateWithIngredientsAsync: {ex.Message}"); return null; }
    }

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
        catch (Exception ex) { Debug.WriteLine($"Exception UpdateAsync: {ex.Message}"); return false; }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/recipes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"Exception DeleteAsync: {ex.Message}"); return false; }
    }

    public async Task<IEnumerable<IngredientItem>> GetAllIngredientsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/ingredients");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<IngredientItem>>() ?? new();
            return new List<IngredientItem>();
        }
        catch (Exception ex) { Debug.WriteLine($"Exception GetAllIngredientsAsync: {ex.Message}"); return new List<IngredientItem>(); }
    }

    public async Task<string?> UploadImageAsync(Stream fileStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            // Створюємо контент файлу
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            // Додаємо файл у форму. "file" має збігатися з назвою параметра в ImageController (IFormFile file)
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync("api/image/upload", content);

            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                var jsonNode = System.Text.Json.Nodes.JsonNode.Parse(resultString);
                return jsonNode?["url"]?.ToString();
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Upload image error: {ex.Message}");
            return null;
        }
    }
}