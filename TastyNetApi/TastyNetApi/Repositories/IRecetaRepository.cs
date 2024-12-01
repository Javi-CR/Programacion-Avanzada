using TastyNetApi.Models;

namespace TastyNetApi.Repositories
{
    public interface IRecetaRepository
    {
        bool CrearReceta(RecetaCreateModel receta);
        List<RecipeViewModel> ObtenerRecetasFavoritas(long userId);
    }
}
