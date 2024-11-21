using Microsoft.AspNetCore.Mvc;
using TastyNet.Services;
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
        public IActionResult CrearReceta([FromBody] RecetaCreateModel recetaModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var result = _recetaService.CrearReceta(recetaModel);
            if (!result)
                return StatusCode(500, "Error al crear la receta");

            return Ok(new { Message = "Receta creada exitosamente" });
        }


    }
}
