using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.MaterialAlquiladoDTOs
{
    public class MaterialAlquiladoDetailDTOs
    {
        public MaterialAlquiladoDetailDTOs(int idAlquiler, string? nombre, string? apellidos, string dni,
            string? email, string? telefono, DateTime fechaAlquiler, int precioTotal, MetodoPago metodoPago,
            IList<MaterialAlquiladoItemDTOs> materialesAlquilados)
        {
            IdAlquiler = idAlquiler;
            Nombre = nombre;
            Apellidos = apellidos;
            DNI = dni;
            Email = email;
            Telefono = telefono;
            FechaAlquiler = fechaAlquiler;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            MaterialesAlquilados = materialesAlquilados;
        }

        public int IdAlquiler { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }

        [Required]
        public string DNI { get; set; }

        public string? Email { get; set; }
        public string? Telefono { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAlquiler { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0, int.MaxValue)]
        public int PrecioTotal { get; set; }

        public MetodoPago MetodoPago { get; set; }

        public IList<MaterialAlquiladoItemDTOs> MaterialesAlquilados { get; set; }
    }
}