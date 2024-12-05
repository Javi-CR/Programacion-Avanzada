using TastyNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TastyNet.Servicios
{
    public interface IRecetaService
    {
        Task<string> CrearRecetaAsync(Recipe receta);
        Task<List<RecipeViewModel>> ObtenerRecetasFavoritasAsync(long userId);
    }
}
