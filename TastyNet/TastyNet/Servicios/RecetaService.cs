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

        public async Task<bool> CrearRecetaAsync(RecetaCreateModel receta)
        {
            var json = JsonSerializer.Serialize(receta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Recetas", content);
            return response.IsSuccessStatusCode;
        }
    }
}
