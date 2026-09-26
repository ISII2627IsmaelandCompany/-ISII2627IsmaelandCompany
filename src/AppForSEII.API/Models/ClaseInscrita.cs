//Para organizar donde esta el codigo
namespace AppForSEII.API.Models
{
    
public class ClaseInscrita
{
       //atributos

        [Key]//Primary key
    public int Id { get; set; }

    //public ClaseDeportiva ClaseDeportiva { get; set; }= null!;//no sera nulo
    //public Inscripcion Inscripcion{get;set}= null!;//no sera nulo 
    //IMP => Falta confirmar lo de la FK
    public int ClaseDeportivaId { get; set; }

    public int InscripcionId { get; set; }

    
    //otro error que da al usar esto de las diapositivas
    //[StringLength(50, ErrorMessage = “Comentario no mayor a 50 caracteres.")] //rango maximo de caracterer
    
    public string? Observaciones { get; set; }//puede ser null por la '?'
    [Required]//obligatorio de rellenar campo
    [Range(1, 3, ErrorMessage = "Solo se pueden reservar 3 plazas,2 acompañantes y tu mismo")]
    public int PlazasReservadas { get; set; }

    [Required]
    //Mismo error de data y display
    //[DataType(DataType.Currency)] 
    //[Display(Name = "Price For Renting")]
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