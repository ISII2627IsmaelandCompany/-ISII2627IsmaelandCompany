namespace AppForSEII.API.Models
{
    public class TipoMaterial
    {
        public TipoMaterial()
        {
        }
        public TipoMaterial(int idTipoMaterial, string nombreTipoMaterial)
        {
            IdTipoMaterial = idTipoMaterial;
            NombreTipoMaterial = nombreTipoMaterial;
        }
    
        
        [Range(1, 999, ErrorMessage = "El ID debe estar entre 1 y 999.")]
        public int IdTipoMaterial { get; set; }


        [StringLength(50, ErrorMessage = "El nombre del tipo de material no puede tener más de 50 carácteres.")]
        public string NombreTipoMaterial { get; set; }
    }
}