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

}
 
