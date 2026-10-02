namespace AppForSEII.API.Models
{
    public class Alquiler
    {
        public Alquiler()
        {
        }
        public Alquiler(DateTime fechaAlquiler, int idAlquiler, List<MaterialAlquilado> materialesAlquilados, MetodoPago metodoPago, int precioTotal)
        {
           FechaAlquiler = DateTime.Now;
           IdAlquiler = idAlquiler;
           MaterialesAlquilados = materialesAlquilados;
           MetodoPago = (MetodoPago) metodoPago;
           PrecioTotal = precioTotal;
        }
    
        [Key]
        [Range(1, 999, ErrorMessage = "El ID del alquiler debe estar entre 1 y 999.")]
        public int IdAlquiler { get; set; }

        [Required(ErrorMessage = "La fecha de alquiler es obligatoria.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaAlquiler { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [StringLength(15, ErrorMessage = "El método de pago no puede tener más de 15 caracteres.", MinimumLength = 3)]
        public MetodoPago MetodoPago { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio total no puede ser negativo.")]
        public int PrecioTotal { get; set; }
        public List<MaterialAlquilado> MaterialesAlquilados { get; set; } = new List<MaterialAlquilado>();//cardinalidad 1 a muchos con MaterialAlquilado
    }
}