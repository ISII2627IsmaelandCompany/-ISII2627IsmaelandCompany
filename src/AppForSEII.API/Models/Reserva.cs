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



    


    //constructores 
    public Reserva()
    {
    }
    public Reserva(MetodoPago metodoPago, DateTime fechaReserva, decimal precioTotal)
    {
        MetodoPago = metodoPago;
        FechaReserva = fechaReserva;
        PrecioTotal = precioTotal;
    }
}
}