public class CompeticionInscripcion
{
    [Key]
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la competición debe ser mayor o igual a 1")]
    public int CompeticionId { get; set; }
    [Key]
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la inscripción debe ser mayor o igual a 1")]
    public int InscripcionId { get; set; }
    [StringLength(200, ErrorMessage = "Los problemas físicos no pueden tener más de 200 caracteres")]
    public string? ProblemasFisicos { get; set; }

    public Competicion? Competicion { get; set; } // Relación con Competicion, dada su cardinalidad de 1 a muchos, se puede acceder a la propiedad Competicion desde CompeticionInscripcion.
    public List<Inscripcion>? Inscripciones { get; set; } // Relación con Inscripcion, dada su cardinalidad de 1 a muchos, se puede acceder a la propiedad Inscripciones desde CompeticionInscripcion.
}