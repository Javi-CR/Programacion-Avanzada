using System.Net.Http;
using System.Text;
using System.Text.Json;
using TastyNet.Models;

namespace TastyNet.Servicios
{

    public class RecetaService : IRecetaService
    {
        private readonly HttpClient _httpClient;

        public RecetaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CrearRecetaAsync(Recipe receta)
        {
            var json = JsonSerializer.Serialize(receta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Recetas/CrearReceta", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear la receta: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CreateRecipeResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Message ?? "Receta creada exitosamente.";
        }

        public async Task<List<RecipeViewModel>> ObtenerRecetasFavoritasAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"/api/Recetas/ObtenerRecetasFavoritas?userId={userId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener recetas favoritas.");

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RecipeViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        public async Task<string> EliminarRecetaAsync(long recipeId)
        {
            var response = await _httpClient.DeleteAsync($"/api/Recetas/EliminarReceta/{recipeId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al eliminar la receta: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<EliminarRecetaResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Message ?? "Receta eliminada exitosamente.";
        }



    }

    public class CreateRecipeResponse
    {
        public string Message { get; set; }
    }


    public class EliminarRecetaResponse
    {
        public string Message { get; set; }
    }
}
