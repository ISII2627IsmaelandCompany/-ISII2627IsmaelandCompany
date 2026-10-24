
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII.API.DTOs.InscripcionDTOs
{
    public class InscripcionDetailDTO//sirve para mostrar la información de una inscripción en detalle, incluyendo los datos del usuario y las competiciones en las que está inscrito
    {
        public InscripcionDetailDTO(//sirve para mostrar la información de una inscripción en detalle, incluyendo los datos del usuario y las competiciones en las que está inscrito
            int id,
            string? nombre,
            string? apellidos,
            string dni,
            string? email,
            string? telefono,
            DateTime fechaInscripcion,
            decimal precioTotal,
            MetodoPago metodoPago,
            IList<InscripcionItemDTO> competicionesInscritas)//sirve para mostrar la información de una inscripción en detalle, incluyendo los datos del usuario y las competiciones en las que está inscrito
        {
            Id = id;
            Nombre = nombre;
            Apellidos = apellidos;
            DNI = dni;
            Email = email;
            Telefono = telefono;
            FechaInscripcion = fechaInscripcion;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            CompeticionesInscritas = competicionesInscritas;
        }

        public int Id { get; set; }

        public string? Nombre { get; set; }

        public string? Apellidos { get; set; }

        [Required]
        public string DNI { get; set; }

        public string? Email { get; set; }

        public string? Telefono { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaInscripcion { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, 9999999999.99)]
        public decimal PrecioTotal { get; set; }

        public MetodoPago MetodoPago { get; set; }

        public IList<InscripcionItemDTO> CompeticionesInscritas { get; set; }
    }
}
