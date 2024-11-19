using System.Collections.Generic;
using System.Threading.Tasks;
using TastyNetApi.Models.DTOs;

namespace TastyNestApi.Data.Repositories
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<RecipeDto>> GetRecipesAsync(string? search, long? category);
        Task<RecipeDetailsDto?> GetRecipeByIdAsync(long id);
        Task<long> CreateRecipeAsync(CreateRecipeDto recipeDto);
    }
}
