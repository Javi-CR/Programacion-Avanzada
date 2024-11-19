using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using TastyNestApi.Services.Interfaces;
using TastyNetApi.Models.DTOs;

namespace TastyNestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        /// <summary>
        /// Obtiene una lista de recetas con filtros opcionales.
        /// </summary>
        /// <param name="search">Texto para buscar en nombre o descripción.</param>
        /// <param name="category">ID de la categoría para filtrar.</param>
        /// <returns>Lista de recetas.</returns>
        [HttpGet]
        public async Task<IActionResult> GetRecipes([FromQuery] string? search, [FromQuery] long? category)
        {
            var recipes = await _recipeService.GetRecipesAsync(search, category);
            return Ok(recipes);
        }

        /// <summary>
        /// Obtiene los detalles de una receta específica por su ID.
        /// </summary>
        /// <param name="id">ID de la receta.</param>
        /// <returns>Detalles completos de la receta.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById(long id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
                return NotFound(new { message = "Recipe not found." });

            return Ok(recipe);
        }

        /// <summary>
        /// Crea una nueva receta.
        /// </summary>
        /// <param name="recipeDto">Datos de la receta a crear.</param>
        /// <returns>La receta creada.</returns>
        [HttpPost]
        [Authorize] // Asegúrate de manejar autenticación y autorización
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto recipeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (recipeDto == null)
                return BadRequest("Recipe data cannot be null.");

            try
            {
                var recipeId = await _recipeService.CreateRecipeAsync(recipeDto);
                return CreatedAtAction(nameof(GetRecipeById), new { id = recipeId }, null);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(500, new { message = "An error occurred while creating the recipe.", details = ex.Message });
            }
        }
    }
}
