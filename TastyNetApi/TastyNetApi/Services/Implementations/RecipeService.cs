using System.Collections.Generic;
using System.Threading.Tasks;
using TastyNestApi.Data.Repositories;
using TastyNestApi.Services.Interfaces;
using TastyNetApi.Models.DTOs;

namespace TastyNestApi.Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<RecipeDto>> GetRecipesAsync(string? search, long? category)
        {
            return await _recipeRepository.GetRecipesAsync(search, category);
        }

        public async Task<RecipeDetailsDto?> GetRecipeByIdAsync(long id)
        {
            return await _recipeRepository.GetRecipeByIdAsync(id);
        }

        public async Task<long> CreateRecipeAsync(CreateRecipeDto recipeDto)
        {
            return await _recipeRepository.CreateRecipeAsync(recipeDto);
        }
    }
}
