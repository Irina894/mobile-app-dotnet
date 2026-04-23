using System.Net.Http.Json;
using WhatToCook.MAUI.Models;

namespace WhatToCook.MAUI.Services
{
    public class RecipeApiService : IRecipeApiService
    {
        private readonly HttpClient _httpClient;

        public RecipeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<RecipeModel>> GetAllAsync()
        {
            try
            {
                var recipes = await _httpClient.GetFromJsonAsync<List<RecipeModel>>("api/recipes");
                return recipes ?? new List<RecipeModel>();
            }
            catch
            {
                return new List<RecipeModel>();
            }
        }

        public async Task<RecipeModel?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<RecipeModel>($"api/recipes/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<RecipeModel?> CreateAsync(RecipeModel recipe)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/recipes", recipe);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<RecipeModel>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(RecipeModel recipe)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/recipes/{recipe.Id}", recipe);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/recipes/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}