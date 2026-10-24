
using System;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII.API.DTOs.InscripcionDTOs
{
    public class InscripcionItemDTO//sirve para mostrar la información de una inscripción en la lista de inscripciones de un usuario, sin necesidad de mostrar todos los detalles de la inscripción
    {
        public InscripcionItemDTO(string nombre, string lugar, DateTime fecha, decimal precio, string? problemasFisicos)
        {
            Nombre = nombre;
            Lugar = lugar;
            Fecha = fecha;
            Precio = precio;
            ProblemasFisicos = problemasFisicos;
        }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Lugar { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, 9999999999.99)]
        public decimal Precio { get; set; }

        [StringLength(200)]
        public string? ProblemasFisicos { get; set; }
    }
}
