namespace AppForSEII.API.Models
{
    public class TipoMaterial
    {
        public TipoMaterial()
        {
        }

        public TipoMaterial(int idTipoMaterial, string nombreTipoMaterial, List<Material> materiales = null!)
        {
            IdTipoMaterial = idTipoMaterial;
            NombreTipoMaterial = nombreTipoMaterial;
            Materiales = materiales ?? new List<Material>(); //por si la lista es nula
        }

        [Key]
        [Range(1, 999, ErrorMessage = "El ID debe estar entre 1 y 999.")]
        public int IdTipoMaterial { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de material es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del tipo de material no puede tener más de 50 caracteres.")]
        public string NombreTipoMaterial { get; set; } = string.Empty;

        public List<Material> Materiales { get; set; } //cardinalidad 1 a muchos con Material!!!
    }
}