namespace AppForSEII.API.DTOs.InscripcionDTOs
{
   public class InscripcionDetailDTO
    {
        public InscripcionDetailDTO(int id, string? nombre, string? apellidos, string dni, string? email, string? telefono, DateTime fechaInscripcion, decimal precioTotal, MetodoPago metodoPago, IList<InscripcionItemDTO> clasesInscritas)
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
            ClasesInscritas = clasesInscritas;
        }

        public int Id{get; set;}

        public string? Nombre{get; set;}

        public string? Apellidos{get; set;}

        [Required]
        public string DNI{get; set;}

        public string? Email{get; set;}

        public string? Telefono{get; set;}

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaInscripcion{get; set;}

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0, 999.99)]
        public decimal PrecioTotal{get; set;}

        public MetodoPago MetodoPago{get; set;}

        public IList<InscripcionItemDTO> ClasesInscritas {get; set;}

      
    }
}