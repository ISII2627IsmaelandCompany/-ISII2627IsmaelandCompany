using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.MaterialAlquiladoDTOs
{
    // Cada línea que envía el cliente: qué material y cuántas unidades
    public class MaterialAlquiladoItemForCreateDTO
    {
        public MaterialAlquiladoItemForCreateDTO() { }

        public MaterialAlquiladoItemForCreateDTO(int materialId, int cantidad)
        {
            MaterialId = materialId;
            Cantidad = cantidad;
        }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        public int Cantidad { get; set; }
    }

    // DTO que recibe el POST
    public class MaterialAlquiladoForCreateDTO
    {
        public MaterialAlquiladoForCreateDTO() { }

        public MaterialAlquiladoForCreateDTO(string clienteUserName, DateTime fechaAlquiler,
            MetodoPago metodoPago, string? descripcion,
            IList<MaterialAlquiladoItemForCreateDTO> materiales)
        {
            ClienteUserName = clienteUserName ?? throw new ArgumentNullException(nameof(clienteUserName));
            FechaAlquiler = fechaAlquiler;
            MetodoPago = metodoPago;
            Descripcion = descripcion;
            Materiales = materiales ?? throw new ArgumentNullException(nameof(materiales));
        }

        [EmailAddress]
        [Required]
        public string ClienteUserName { get; set; } = string.Empty; // correo del cliente

        [Required(ErrorMessage = "Debe introducir la fecha del alquiler")]
        public DateTime FechaAlquiler { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Tarjeta;

        [StringLength(500)]
        public string? Descripcion { get; set; } // opcional: qué va a hacer con el material

        public IList<MaterialAlquiladoItemForCreateDTO> Materiales { get; set; }
            = new List<MaterialAlquiladoItemForCreateDTO>();
    }
}