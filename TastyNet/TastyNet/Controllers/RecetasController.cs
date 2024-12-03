using Microsoft.AspNetCore.Mvc;
using TastyNet.Servicios;
using TastyNet.Models;

namespace TastyNet.Controllers
{
    [Route("Recetas")]
    [ApiController]
    public class RecetasController : Controller
    {
        private readonly IRecetaService _recetaService;

        public RecetasController(IRecetaService recetaService)
        {
            _recetaService = recetaService;
        }

        [HttpPost("CrearReceta")]
        public async Task<IActionResult> CrearReceta([FromBody] Recipe model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _recetaService.CrearRecetaAsync(model);
                if (!result)
                    return StatusCode(500, "Error al crear la receta");

                return Ok(new { Message = "Receta creada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("ObtenerRecetasFavoritas")]
        public async Task<IActionResult> ObtenerRecetasFavoritas()
        {
            var userId = 1; // Usuario fijo por ahora
            var recetas = await _recetaService.ObtenerRecetasFavoritasAsync(userId);
            return Json(recetas);
        }
    }
}
