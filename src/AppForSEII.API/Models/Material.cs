namespace AppForSEII.API.Models
{
    public class Material
    {
        public Material()
        {
        }
        public Material(int cantidad, int idMaterial, string nombre, decimal precio)
        {
            Cantidad = cantidad;
            IdMaterial = idMaterial;
            Nombre = nombre;
            Precio = precio;
        }
        [Key]
        [Range(1, 999, ErrorMessage = "El ID debe estar entre 1 y 999.")]
        public int IdMaterial { get; set; }


        [StringLength(50, ErrorMessage = "El nombre del material no puede tener más de 50 carácteres.")]
        public string Nombre { get; set; }


        [Range(0.5, float.MaxValue, ErrorMessage = "El precio mínimo es 0.5 ")]
        public decimal Precio { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser como mínimo 1.")]
        public int Cantidad { get; set; }
    }
}