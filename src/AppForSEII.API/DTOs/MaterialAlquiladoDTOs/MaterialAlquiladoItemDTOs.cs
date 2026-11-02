using System;

namespace AppForSEII.API.DTOs.MaterialAlquiladoDTOs
{
    // DTO de cada material alquilado: una fila de la lista dentro del details
    public class MaterialAlquiladoItemDTOs
    {
        public MaterialAlquiladoItemDTOs() { }  // constructor vacío permite crear el objeto sin datos y rellenarlo después

        // es el que se usa en el Select del controlador
        public MaterialAlquiladoItemDTOs(string nombreMaterial, string tipoMaterial, string tipoDeporte, int cantidad, decimal precio)
        {
            NombreMaterial = nombreMaterial;
            TipoMaterial = tipoMaterial;
            TipoDeporte = tipoDeporte;
            Cantidad = cantidad;
            Precio = precio;
        }

        [Required]
        public string NombreMaterial { get; set; } = string.Empty; //para que no salga el Nullexception
        
        [Required]
        public string TipoMaterial { get; set; } = string.Empty;
        
        [Required]
        public string TipoDeporte { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser como mínimo 1.")]
        public int Cantidad { get; set; }
        
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] //Para indicar que es dinero
        [Range(0,999.99)]   //Para poner un rango de precio
        public decimal Precio { get; set; }

        // el equals define cuando dos DTOs son iguales
        public override bool Equals(object? obj)
        {
            return obj is MaterialAlquiladoItemDTOs dto &&
                   NombreMaterial == dto.NombreMaterial &&
                   TipoMaterial == dto.TipoMaterial &&
                   TipoDeporte == dto.TipoDeporte &&
                   Cantidad == dto.Cantidad &&
                   Precio == dto.Precio;
        }

        // el GetHashCode siempre se sobrescribe junto a Equals, dos objetos iguales deben dar el mismo hash
        public override int GetHashCode()
        {
            return HashCode.Combine(NombreMaterial, TipoMaterial, TipoDeporte, Cantidad, Precio);
        }
    }
}