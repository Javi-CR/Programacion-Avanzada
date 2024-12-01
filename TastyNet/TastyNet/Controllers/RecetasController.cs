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
        public async Task<IActionResult> CrearReceta([FromBody] RecetaCreateModel model)
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

        // Metodo para cargar las recetas favoritas del usuario
        [HttpGet]
        public async Task<IActionResult> ObtenerRecetasFavoritas()
        {
            var userId = 1; 
            var recetas = await _recetaService.ObtenerRecetasFavoritasAsync(userId);
            return Json(recetas);
        }





    }
}
