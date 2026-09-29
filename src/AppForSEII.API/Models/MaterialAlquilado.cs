namespace AppForSEII.API.Models
{
    public class MaterialAlquilado
    {
        public MaterialAlquilado()
        {
        }
        public MaterialAlquilado(int cantidad, string descripcion, int idAlquiler, int idMaterial, decimal precio)
        {
            Cantidad = cantidad;
            Descripcion = descripcion;
            IdAlquiler = idAlquiler;
            IdMaterial = idMaterial;
            Precio = precio;
        }
        
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser como mínimo 1.")]
        public int Cantidad { get; set; }


        [StringLength(250, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 250 caracteres.")]
        public string Descripcion { get; set; }


        [Range(1, 999, ErrorMessage = "El ID debe estar entre 1 y 999.")]
        public int IdMaterial { get; set; }

        [Key]
        [Range(1, 999, ErrorMessage = "El ID debe estar entre 1 y 999.")]
        public int IdAlquiler { get; set; }


        [Range(0.5, float.MaxValue, ErrorMessage = "El precio mínimo es 0.5 ")]
        public decimal Precio { get; set; }
    }
}