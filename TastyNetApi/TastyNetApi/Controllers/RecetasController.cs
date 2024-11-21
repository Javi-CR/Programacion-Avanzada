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

        [HttpPost]
        public IActionResult CrearReceta([FromBody] RecetaCreateModel receta)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var result = _recetaRepository.CrearReceta(receta);
            if (!result)
                return StatusCode(500, "Error al crear la receta");

            return Ok(new { Message = "Receta creada exitosamente" });
        }
    }
}
