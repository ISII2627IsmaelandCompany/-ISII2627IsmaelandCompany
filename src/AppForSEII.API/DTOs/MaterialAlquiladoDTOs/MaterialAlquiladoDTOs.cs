using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII.API.DTOs.MaterialAlquiladoDTOs //namespace AppForSEII.API.DTOs.MaterialAlquiladoDTOs creando el folder de la DTO en concreto
{
    public class MaterialAlquiladoDTO
    {
        public MaterialAlquiladoDTO() //constructor vacío
        {
        }

        public MaterialAlquiladoDTO(int cantidad, string descripcion, int idMaterial, int idAlquiler, decimal precio)
        {
            Cantidad = cantidad;
            Descripcion = descripcion;
            IdMaterial = idMaterial;
            IdAlquiler = idAlquiler;
            Precio = precio;
        }

        [Required] 
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser como mínimo 1.")]
        public int Cantidad { get; set; } = 1;

        [Required]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 250 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del material debe ser válido.")]
        public int IdMaterial { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del alquiler debe ser válido.")]
        public int IdAlquiler { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, double.MaxValue, ErrorMessage = "El precio mínimo es 0.5")]
        public decimal Precio { get; set; } = 0.5m;

        public override bool Equals(object? obj) //comprueba el tipo de obejeto y compara las propiedades de la clase MaterialAlquiladoDTO con las del objeto pasado como parámetro
        {
            return obj is MaterialAlquiladoDTO dTO &&
                   Cantidad == dTO.Cantidad &&
                   Descripcion == dTO.Descripcion &&
                   IdMaterial == dTO.IdMaterial &&
                   IdAlquiler == dTO.IdAlquiler &&
                   Precio == dTO.Precio;
        }

        public override int GetHashCode() //devuelve un valor hash para el objeto actual, basado en las propiedades de la clase MaterialAlquiladoDTO
        {
            return HashCode.Combine(Cantidad, Descripcion, IdMaterial, IdAlquiler, Precio);
        }
    }
}