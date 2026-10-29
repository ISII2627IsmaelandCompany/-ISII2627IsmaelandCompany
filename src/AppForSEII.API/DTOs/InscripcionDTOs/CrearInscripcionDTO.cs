
using AppForSEII.API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.InscripcionDTOs
{
    public class CrearInscripcionDTO//esta dto sirve para recibir los datos necesarios para realizar una inscripción.
    {
        [Required]
        public string NombreUsuarioCliente { get; set; } = string.Empty;

        [Required]
        public MetodoPago MetodoPago { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos una competición.")]
        public IList<CompeticionSeleccionadaDTO> CompeticionesSeleccionadas { get; set; }
            = new List<CompeticionSeleccionadaDTO>();
    }
}
