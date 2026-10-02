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


        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] //tipo de dato precio
        [System.ComponentModel.DataAnnotations.Display(Name = "Precio")] //etiqueta para la pagina web
        [Range(0, 999.99,ErrorMessage = "El precio total no puede ser negativo y máximo 999.99")]//rango de precio
        [Precision(5, 2)]
        public decimal Precio { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser como mínimo 1.")]
        public int Cantidad { get; set; }

        public TipoDeporte TipoDeporte { get; set; } = null!; //el null! es para mas adelante para la hora de hacer la relacion entre las clases
        public int TipoDeporteId { get; set; }// FK que relaciona ClaseDeportiva con TipoDeporte de  1 a N

        public TipoMaterial TipoMaterial { get; set; } = null!;
        public int TipoMaterialId { get; set; }// FK que relaciona ClaseDeportiva con TipoMaterial de  1 a N


        public List<MaterialAlquilado> MaterialesAlquilados {get; set;} = new List<MaterialAlquilado>();
    }
}