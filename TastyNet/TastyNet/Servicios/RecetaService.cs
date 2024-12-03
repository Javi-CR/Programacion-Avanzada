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

        public async Task<bool> CrearRecetaAsync(Recipe receta)
        {
            var json = JsonSerializer.Serialize(receta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Recetas/CrearReceta", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<RecipeViewModel>> ObtenerRecetasFavoritasAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"/api/Recetas/ObtenerRecetasFavoritas?userId={userId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener recetas favoritas.");

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RecipeViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
