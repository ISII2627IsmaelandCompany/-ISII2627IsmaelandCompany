public class Pista
{

    
    [Key]
    public int IdPista { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.", MinimumLength = 1)]
    public string NombrePista { get; set; } = string.Empty;//para no tener null

    [Required]
    [Range(1, 30, ErrorMessage = "El minimo de personas es 1 y el maximo es 30")]
    public int NPersonas{get; set;}

    [Required]
    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] //tipo de dato precio
    [System.ComponentModel.DataAnnotations.Display(Name = "Precio")] //etiqueta para la pagina web
    [Range(0, 999.99,ErrorMessage = "El precio total no puede ser negativo y máximo 999.99")]//rango de precio
    [Precision(5, 2)]
    public double Precio{get; set;}

    [Required]
    [Range(0, 999.99,ErrorMessage = "El stock no puede ser negativo y máximo 999.99")]//rango de precio
    public int Stock{get; set;}


    public TipoDeporte TipoDeporte { get; set; } = null!; //el null! es para mas adelante para la hora de hacer la relacion entre las clases
    public int TipoDeporteId { get; set; }// FK que relaciona ClaseDeportiva con TipoDeporte de  1 a N

    //Relacion con PistaReservada
    public IList<PistaReservada> PistasReservadas { get; set; } = new List<PistaReservada>();//Relacion 1--N con PistaReservada. igual que el ejemplo de movie


    //CONSTRUCTORES
    public Pista()
    {
        
    }

    public Pista(string nombrePista, double precio, int nPersonas, int stock)
    {
        this.NombrePista = nombrePista;
        this.Precio = precio;
        this.NPersonas = nPersonas;
        this.Stock = stock;
    }

}
 
