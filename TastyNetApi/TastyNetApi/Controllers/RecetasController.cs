using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TastyNetApi.Models;

namespace TastyNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecetasController : ControllerBase
    {
        private readonly TastyNestDbContext _dbContext;

        public RecetasController(TastyNestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Crea una nueva receta y sus ingredientes y pasos asociados.
        /// </summary>
        [HttpPost("CrearReceta")]
        public async Task<IActionResult> CrearReceta([FromBody] Recipe receta)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            // Establecer un UserId fijo para este ejemplo
            const long userId = 1;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Asignar el UserId fijo a la receta
                receta.UserId = userId;

                // Agregar la receta al contexto
                _dbContext.Recipes.Add(receta);
                await _dbContext.SaveChangesAsync();

                // Asociar los ingredientes con la receta creada
                foreach (var ingredient in receta.Ingredients)
                {
                    ingredient.RecipeId = receta.Id;
                    _dbContext.Ingredients.Add(ingredient);
                }

                // Asociar los pasos con la receta creada
                foreach (var step in receta.RecipeSteps)
                {
                    step.RecipeId = receta.Id;
                    _dbContext.RecipeSteps.Add(step);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Receta creada exitosamente." });
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al crear la receta");
            }
        }

        /// <summary>
        /// Obtiene las recetas favoritas de un usuario fijo (UserId = 1).
        /// </summary>
        [HttpGet("ObtenerRecetasFavoritas")]
        public async Task<IActionResult> ObtenerRecetasFavoritas(long userId)
        {
            var recetasFavoritas = await _dbContext.Favorites
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.Ingredients)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.RecipeSteps)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.Category)
                .Select(f => f.Recipe)
                .ToListAsync();

            return Ok(recetasFavoritas);
        }

    }
}
