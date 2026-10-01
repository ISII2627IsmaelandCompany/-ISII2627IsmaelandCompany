namespace AppForSEII.API.Models
{
public class PistaReservada
{


    
    [Key]
    public int Id {get; set;}
    [Required]

    public int IdPista { get; set; }//  Foreign key
    [Required]

    public int IdReserva{get; set;} //foreing key 


    [Required]
    [Range(1, 30, ErrorMessage = "La cantidad debe estar entre 1 y 30.")]
    public int Cantidad { get; set; }



    [Required]

  [StringLength(50, ErrorMessage = "EL comentario de observacion no puede ser más largo que 50 caracteres.", MinimumLength = 4)]    
    public string? Observaciones {get; set;}//la "?" para que pueda ser null porque son opcionales

    [Required]
    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] //tipo de dato precio
    [System.ComponentModel.DataAnnotations.Display(Name = "Precio")] //etiqueta para la pagina web
    [Range(0, 999.99,ErrorMessage = "El precio total no puede ser negativo y máximo 999.99")]//rango de precio
    [Precision(5, 2)]
    public double Precio {get; set;}






    public Pista? Pista {get; set;} = null! ; //la relacion con la clase pista. le pongo la interrogacion porue supongo que una pista puede tener pistas que no estan reservadas
    public Reserva Reserva {get; set;} = null! ; //relacion con la clase reserva, una reserva puede tener mucha pistasReservadas

    
}
}