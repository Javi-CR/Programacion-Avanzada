using TastyNetApi.Models;

namespace TastyNetApi.Repositories
{
    public interface IRecetaRepository
    {
        bool CrearReceta(RecetaCreateModel receta);
        long CrearRecetaYObtenerId(RecetaCreateModel receta);
        bool AgregarAFavoritos(long userId, long recipeId);
        List<RecipeViewModel> ObtenerRecetasFavoritas(long userId);
    }
}
