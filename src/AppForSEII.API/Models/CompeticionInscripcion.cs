[PrimaryKey(nameof(CompeticionId), nameof(InscripcionId))]
public class CompeticionInscripcion
{
    public CompeticionInscripcion(int competicionId, int inscripcionId, string? problemasFisicos)
    {
        CompeticionId = competicionId;
        InscripcionId = inscripcionId;
        ProblemasFisicos = problemasFisicos;
    }
    public CompeticionInscripcion()
    {
    }
    
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la competición debe ser mayor o igual a 1")]
    public int CompeticionId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El Id de la inscripción debe ser mayor o igual a 1")]
    public int InscripcionId { get; set; }

    [StringLength(200, ErrorMessage = "Los problemas físicos no pueden tener más de 200 caracteres")]
    public string? ProblemasFisicos { get; set; }

    public Competicion Competicion { get; set; } = null!;

    public Inscripcion Inscripcion { get; set; } = null!;
}