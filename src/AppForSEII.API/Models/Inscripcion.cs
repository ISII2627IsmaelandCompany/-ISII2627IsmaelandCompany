namespace AppForSEII.API.Models
{   
   public class Inscripcion
{
    //atributos
    
        [Key]//PK
        public int Id { get; set; }
        public ApplicationUser Cliente { get; set; } = null!;

    //Lo comento y luego lo quito cuando la clase este creada del todo
    //public List<ClaseInscrita> ClasesInscritas { get; set; }
     [Required]//obligatorio
    public string DatosPago { get; set; }=string.Empty;
    [Required]//obligatorio
    //[DataType(DataType.Date), Display(Name ="Release Date")] //error
    //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]// esperar a ver el formato que dicen de usar
    public DateTime FechaInscripcion { get; set; }

   
    
    //IMP => Falta crear/añadir enum MetodoPago
    //public MetodoPago MetodoPago { get; set; }

    [Required]
    [Precision(5, 2)]//precision 5 digitos 2 decimales
    [Range(0, 999.99,ErrorMessage = "El precio total no puede ser negativo y máximo 999.99")]//rango de precio

    public decimal PrecioTotal { get; set; }

    //atributos CU 3
    [Required]
    public string ApellidosUsuario { get; set; } = string.Empty;
    [Required]
    [Range(10000000, 99999999, ErrorMessage = "El DNI debe tener 8 dígitos")]
    public string DNI { get; set; } = string.Empty;
    [Required]
    public string NombreUsuario { get; set; } = string.Empty;
    [Required]
    [Range(600000000, 799999999, ErrorMessage = "El teléfono debe tener 9 dígitos y empezar por 6 o 7")]
    public string Telefono { get; set; } = string.Empty;

    //constructores

    public Inscripcion()
        {
        }
    public Inscripcion(ApplicationUser cliente, string datosPago, DateTime fechaInscripcion, decimal precioTotal, string apellidosUsuario, string dni, string nombreUsuario, string telefono)
    {
        Cliente = cliente;
        DatosPago = datosPago;
        FechaInscripcion = fechaInscripcion;
        PrecioTotal = precioTotal;
        ApellidosUsuario = apellidosUsuario;
        DNI = dni;
        NombreUsuario = nombreUsuario;
        Telefono = telefono;
    }

    


}
    
}