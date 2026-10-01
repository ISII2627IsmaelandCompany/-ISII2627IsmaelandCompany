namespace AppForSEII.API.Models
{   
public class Reserva
{
    //atributos
    [Key] //PK
    public int Id {get; set;}
    
    
    public  IList<PistaReservada> PistasReservadas {get; set;}= new List<PistaReservada>();//Relación 1 a N con PistaReservada

   [Required]//obligatorio
    public MetodoPago MetodoPago { get; set; }//enum con   Bizum, Efectivo,Tarjeta,Transferencia,Metalico

    [Required]//obligatorio
    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    [System.ComponentModel.DataAnnotations.Display(Name ="Fecha Reserva")] 
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]// formato de  fecha
    public DateTime FechaReserva { get; set; }

    
    [Required]
    [Precision(5, 2)]//precision 5 digitos 2 decimales
    [Range(0, 999.99,ErrorMessage = "El precio total no puede ser negativo y máximo 999.99")]//rango de precio
    public decimal PrecioTotal { get; set; }

    [Required]
    public string Apellidos { get; set; } = string.Empty;

    [Required]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    public string NombreCliente { get; set; } = string.Empty;   

    [Required]
    [Range(10000000, 99999999, ErrorMessage = "El DNI debe tener 8 dígitos")]
    public string DNI { get; set; } = string.Empty;

    //constructores 
    public Reserva()
    {
    }
}
}