using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AppForSEII.API.DTOs.MaterialDTOs
{
    public class MaterialDTO
    {
        public MaterialDTO() // Constructor vacío
        {
        }

        public MaterialDTO(int idMaterial, string nombre, decimal precio, int cantidad)
        {
            IdMaterial = idMaterial;
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
        }

        [Required]
        public int IdMaterial { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Cantidad { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is MaterialDTO dTO &&
                   IdMaterial == dTO.IdMaterial &&
                   Nombre == dTO.Nombre &&
                   Precio == dTO.Precio &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdMaterial, Nombre, Precio, Cantidad);
        }
    }
}