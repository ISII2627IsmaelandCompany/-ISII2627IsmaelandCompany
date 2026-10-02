namespace AppForSEII.API.Models
{   
   public class Inscripcion
{

    public Inscripcion()
        {
        }   
    public Inscripcion(ApplicationUser cliente, string datosPago, DateTime fechaInscripcion, MetodoPago metodoPago, decimal precioTotal)
    {
        Cliente = cliente;
        DatosPago = datosPago;
        FechaInscripcion = fechaInscripcion;
        MetodoPago = metodoPago;
        PrecioTotal = precioTotal;
    
    }
    //atributos
    
    [Key]//PK
    public int Id { get; set; }
    public ApplicationUser Cliente { get; set; } // Relación N a 1 con ApplicationUser

    public IList<ClaseInscrita> ClasesInscritas { get; set; }= new List<ClaseInscrita>();//Relación 1 a N con ClaseInscrita
     [Required]//obligatorio
    public string DatosPago { get; set; }=string.Empty;
    [Required]//obligatorio
    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    [System.ComponentModel.DataAnnotations.Display(Name ="Fecha Inscripcion")] 
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]// formato de  fecha
    public DateTime FechaInscripcion { get; set; }

   
    
    
    public MetodoPago MetodoPago { get; set; }//enum con   Bizum, Efectivo,Tarjeta,Transferencia,Metalico
   

    [Required]
    [Precision(5, 2)]//precision 5 digitos 2 decimales
    [Range(0, 999.99,ErrorMessage = "El precio total no puede ser negativo y máximo 999.99")]//rango de precio
    public decimal PrecioTotal { get; set; }

    //atributos CU 3

    public List<CompeticionInscripcion> CompeticionInscripciones { get; set; } = new();
    //constructores
    
}
    
}