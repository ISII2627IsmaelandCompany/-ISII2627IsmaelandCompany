
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.InscripcionDTOs
{
    public class CompeticionSeleccionadaDTO//Esta DTO sirve para indicar qué competición se selecciona y los posibles problemas físicos.
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "El identificador de la competición debe ser mayor que 0.")]
        public int IdCompeticion { get; set; }

        [StringLength(200,
            ErrorMessage = "Los problemas físicos no pueden superar los 200 caracteres.")]
        public string? ProblemasFisicos { get; set; }
    }
}
