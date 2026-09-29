//Para organizar donde esta el codigo
namespace AppForSEII.API.Models
{
    
public class ClaseInscrita
{
       //atributos

        [Key]//Primary key
    public int Id { get; set; }

    //public ClaseDeportiva ClaseDeportiva { get; set; }= null!;//no sera nulo
    public Inscripcion Inscripcion{get;set;}= null!;//no sera nulo 
    // FK que relaciona ClaseInscrita con ClaseDeportiva
    public int ClaseDeportivaId { get; set; }
    // FK que relaciona ClaseInscrita con Inscripcion
    public int InscripcionId { get; set; }

    
    //otro error que da al usar esto de las diapositivas
    [StringLength(50, ErrorMessage = "EL comentario de observacion no puede ser más largo que50  caracteres.", MinimumLength = 4)]    
    public string? Observaciones { get; set; }//puede ser null por la '?'
    [Required]//obligatorio de rellenar campo
    [Range(1, 3, ErrorMessage = "Solo se pueden reservar 3 plazas,2 acompañantes y tu mismo")]
    public int PlazasReservadas { get; set; }
    
    [Required]
    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] //tipo de dato precio
    [System.ComponentModel.DataAnnotations.Display(Name = "Precio")] //etiqueta para la pagina web
    [Range(0, 999.99, ErrorMessage = "El precio no puede ser negativo y máximo 999.99")]
    [Precision(5, 2)]
    public decimal Precio { get; set; }
    

    //constructores
    public ClaseInscrita()
        {
            

            
        }

        public ClaseInscrita( string? observaciones, int plazasReservadas, decimal precio)
        {
            this.Observaciones = observaciones;
            this.PlazasReservadas = plazasReservadas;
            this.Precio = precio;
        }
    }




    
}