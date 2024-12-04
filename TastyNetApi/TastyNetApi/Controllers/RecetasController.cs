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
                return BadRequest(ModelState);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                
                receta.UserId = 1; 

                receta.Ingredients ??= new List<Ingredient>();
                receta.RecipeSteps ??= new List<RecipeStep>();

                _dbContext.Recipes.Add(receta);
                await _dbContext.SaveChangesAsync();

                foreach (var ingredient in receta.Ingredients)
                {
                    ingredient.RecipeId = receta.Id;
                }

                foreach (var step in receta.RecipeSteps)
                {
                    step.RecipeId = receta.Id;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Receta creada exitosamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al crear la receta: {ex.Message}");
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
