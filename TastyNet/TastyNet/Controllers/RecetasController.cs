using Microsoft.AspNetCore.Mvc;
using TastyNet.Servicios;
using TastyNet.Models;

namespace TastyNet.Controllers
{
    public class RecetasController : Controller
    {

        private readonly IRecetaService _recetaService;

        public RecetasController(IRecetaService recetaService)
        {
            _recetaService = recetaService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearReceta([FromBody] RecetaCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var result = await _recetaService.CrearRecetaAsync(model); 
            if (!result)
                return StatusCode(500, "Error al crear la receta");

            return Ok(new { Message = "Receta creada exitosamente" });
        }


    }
}
