namespace AppForSEII.API.Models
{
    public class Alquiler
    {
        public Alquiler()
        {
        }
        public Alquiler(String apellidoUsuario, String dni, DateTime fechaAlquiler, int idAlquiler, List<MaterialAlquilado> materialesAlquilados, MetodoPago metodoPago, string nombreUsuario, int numeroTelefono, int precioTotal)
        {
           ApellidoUsuario = apellidoUsuario;
           DNI = dni;
           FechaAlquiler = DateTime.Now;
           IdAlquiler = idAlquiler;
           MaterialesAlquilados = materialesAlquilados;
           MetodoPago = (MetodoPago) metodoPago;
           NombreUsuario = nombreUsuario;
           NumeroTelefono = numeroTelefono;
           PrecioTotal = precioTotal;
        }
    
        [Key]
        [Range(1, 999, ErrorMessage = "El ID del alquiler debe estar entre 1 y 999.")]
        public int IdAlquiler { get; set; }

        [Required(ErrorMessage = "El nombre del usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del usuario no puede tener más de 50 caracteres.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El apellido del usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido del usuario no puede tener más de 50 caracteres.")]
        public string ApellidoUsuario { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        public string DNI { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        public int NumeroTelefono { get; set; }

        [Required(ErrorMessage = "La fecha de alquiler es obligatoria.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaAlquiler { get; set; }

        public List<MaterialAlquilado> MaterialesAlquilados { get; set; } //cardinalidad 1 a muchos con MaterialAlquilado

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [StringLength(15, ErrorMessage = "El método de pago no puede tener más de 15 caracteres.", MinimumLength = 3)]
        public MetodoPago MetodoPago { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio total no puede ser negativo.")]
        public int PrecioTotal { get; set; }
    }
}