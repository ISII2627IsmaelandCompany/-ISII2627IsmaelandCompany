public class Competicion
{

    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime Fecha { get; set; } 
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "El lugar no puede tener más de 100 caracteres")]
    public string Lugar { get; set; } = string.Empty;
    [Required]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    [Range (0, int.MaxValue, ErrorMessage = "Las plazas deben ser mayor o igual a 0")]
    public int Plazas { get; set; }

    [Required]
    [Range (0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    [Precision(10, 2)]
    public decimal Precio { get; set; }

    public TipoDeporte? TipoDeporte { get; set; }// Relación con TipoDeporte, dada su cardinalidad de 1 a muchos, se puede acceder a la propiedad TipoDeporte desde Competicion.    
    public List<CompeticionInscripcion> CompeticionInscripciones { get; set; } = new List<CompeticionInscripcion>();// Relación con CompeticionInscripcion, dada su cardinalidad de 1 a 1 o muchos, se puede acceder a cada CompeticionInscripciones desde Competicion.


}
