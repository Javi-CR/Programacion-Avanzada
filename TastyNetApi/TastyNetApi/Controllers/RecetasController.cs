using Microsoft.AspNetCore.Mvc;
using TastyNetApi.Models;
using TastyNetApi.Repositories;


namespace TastyNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecetasController : ControllerBase
    {
        private readonly IRecetaRepository _recetaRepository;

        public RecetasController(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        [HttpPost("CrearReceta")]
        public IActionResult CrearReceta([FromBody] RecetaCreateModel receta)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            // Crear la receta y obtener el ID generado
            var recipeId = _recetaRepository.CrearRecetaYObtenerId(receta);
            if (recipeId <= 0)
                return StatusCode(500, "Error al crear la receta");

            // Agregar la receta a favoritos
            var favoritoAgregado = _recetaRepository.AgregarAFavoritos(1, recipeId); // ID de usuario fijo por ahora
            if (!favoritoAgregado)
                return StatusCode(500, "Error al agregar la receta a favoritos");

            return Ok(new { Message = "Receta creada exitosamente y añadida a favoritos" });
        }


        [HttpGet("ObtenerRecetasFavoritas")]
        public IActionResult ObtenerRecetasFavoritas(long userId)
        {
            var recetas = _recetaRepository.ObtenerRecetasFavoritas(userId);

            // esto es para validacion y limpieza de datos nulos si los hay
            var recetasLimpias = recetas.Select(r => new RecipeViewModel
            {
                Id = r.Id,
                Name = r.Name ?? "Sin nombre",
                Category = r.Category ?? "Sin categoría",
                Ingredients = r.Ingredients
                    .Where(i => !string.IsNullOrWhiteSpace(i.Name) && !string.IsNullOrWhiteSpace(i.Quantity))
                    .ToList(),
                Steps = r.Steps
                    .Where(s => !string.IsNullOrWhiteSpace(s.Description))
                    .ToList()
            }).ToList();

            if (recetasLimpias == null || !recetasLimpias.Any())
                return Ok(new List<RecipeViewModel>()); // Retorna lista vacía si no hay recetas.

            return Ok(recetasLimpias);
        }



    }
}
