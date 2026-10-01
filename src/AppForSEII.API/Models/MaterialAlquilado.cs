namespace AppForSEII.API.Models
{
    [PrimaryKey(nameof(IdMaterial), nameof(IdAlquiler))]
    public class MaterialAlquilado
    {
        public MaterialAlquilado()
        {
        }
        public MaterialAlquilado(int cantidad, string descripcion, int idMaterial, int idAlquiler, decimal precio)
        {
            Cantidad = cantidad;
            Descripcion = descripcion;
            IdMaterial = idMaterial;
            IdAlquiler = idAlquiler;
            Precio = precio;
        }
        
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser como mínimo 1.")]
        public int Cantidad { get; set; }


        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 250 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El ID del material es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del material debe ser válido.")]
        public int IdMaterial { get; set; }

        [Required(ErrorMessage = "El ID del alquiler es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del alquiler debe ser válido.")]
        public int IdAlquiler { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.5, float.MaxValue, ErrorMessage = "El precio mínimo es 0.5 ")]
        public decimal Precio { get; set; }

        public Alquiler Alquiler { get; set; }
        public Material Material { get; set; }
    }
}