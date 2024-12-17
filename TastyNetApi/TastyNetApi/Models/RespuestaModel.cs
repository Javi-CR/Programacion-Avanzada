using System.ComponentModel.DataAnnotations.Schema;

namespace TastyNetApi.Models
{
    [NotMapped]
    public class Respuesta
    {
    public int Codigo { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public object? Contenido { get; set; }

    }
}
