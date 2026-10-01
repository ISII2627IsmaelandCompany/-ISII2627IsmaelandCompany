[PrimaryKey(nameof(CompeticionId), nameof(InscripcionId))] // Se establece la clave primaria compuesta por CompeticionId e InscripcionId
public class CompeticionInscripcion
{
 
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la competición debe ser mayor o igual a 1")]
    public int CompeticionId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la inscripción debe ser mayor o igual a 1")]
    public int InscripcionId { get; set; }
    [StringLength(200, ErrorMessage = "Los problemas físicos no pueden tener más de 200 caracteres")]
    public string? ProblemasFisicos { get; set; }

    public Competicion Competiciones { get; set; } = null!; // Relación con Competicion, dada su cardinalidad de 1 a muchos, se puede acceder a la propiedad Competicion desde CompeticionInscripcion.
    public Inscripcion Inscripciones { get; set; } = null!;  // Relación con Inscripcion, dada su cardinalidad de 1 a muchos, se puede acceder a la propiedad Inscripciones desde CompeticionInscripcion.
}