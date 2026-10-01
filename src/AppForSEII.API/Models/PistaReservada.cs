namespace AppForSEII.API.Models
{
  [PrimaryKey(nameof(IdPista), nameof(IdReserva))]//PK compuesta como en el ejemplo de purchase-PURCHASEITEM-MOVIE

public class PistaReservada
{
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


    public Pista Pista {get; set;} = null!; //la relacion con la clase pista.
    public Reserva Reserva {get; set;} = null! ; //relacion con la clase reserva, una reserva puede tener mucha pistasReservadas

    [Required]
    public int IdPista { get; set; }//  Foreign key
    [Required]

    public int IdReserva{get; set;} //foreing key 


    //CONSTRUCTORES
    public PistaReservada()
    {
        
    }
    public PistaReservada(Pista pista, int cantidad, double precio)
    {
        Pista = pista;
        IdPista = Pista.IdPista;
        IdReserva = Reserva.Id;
        this.Cantidad = cantidad;
        this.Precio = precio;
    }   
}
}