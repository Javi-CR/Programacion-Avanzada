using TastyNet.Models;

namespace TastyNet.Servicios
{
    public interface IRecetaService
    {
        Task<bool> CrearRecetaAsync(RecetaCreateModel receta);
    }
}


