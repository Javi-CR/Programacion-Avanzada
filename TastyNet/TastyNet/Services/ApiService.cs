using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TastyNet.Models;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("TastyNestApi");
    }

    public async Task<List<RecipeViewModel>> GetRecipesAsync(string? search = null, long? category = null)
    {
        var url = "api/recipes";
        if (!string.IsNullOrWhiteSpace(search) || category.HasValue)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={search}");
            if (category.HasValue) queryParams.Add($"category={category}");
            url += "?" + string.Join("&", queryParams);
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<RecipeViewModel>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<RecipeDetailsViewModel> GetRecipeDetailsAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/recipes/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RecipeDetailsViewModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task CreateRecipeAsync(CreateRecipeViewModel recipe)
    {
        var jsonRequest = JsonSerializer.Serialize(recipe);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/recipes", content);
        response.EnsureSuccessStatusCode();
    }
}
