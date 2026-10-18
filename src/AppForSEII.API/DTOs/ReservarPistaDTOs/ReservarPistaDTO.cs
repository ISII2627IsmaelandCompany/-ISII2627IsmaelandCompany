using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppForSEII.API.DTOs.ReservarPistaDTOs
{
    public class ReservarPistaDTO
    {
        public ReservarPistaDTO()
        {
        }

        public ReservarPistaDTO(int idPista, string nombrePista, int nPersonas, double precio, int stock)
        {
            IdPista = idPista;
            NombrePista = nombrePista;
            NPersonas = nPersonas;
            Precio = precio;
            Stock = stock;
        }

        public int IdPista { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.", MinimumLength = 1)]
        public string NombrePista { get; set; } = string.Empty;//para no tener null

        [Required]
        [Range(1, 30, ErrorMessage = "El número de personas debe ser un valor positivo y como mucho 30.")]
        public int NPersonas { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] //tipo de dato precio
        [System.ComponentModel.DataAnnotations.Display(Name = "Precio")] //etiqueta para la pagina web
        [Range(0, 999.99, ErrorMessage = "El precio total no puede ser negativo y máximo 999.99")]
        [Precision(5, 2)]
        public double Precio { get; set; }

        [Required]
        [Range(0, 999.99, ErrorMessage = "El stock no puede ser negativo y máximo 999.99")]//rango de precio    
        public int Stock { get; set; }
    
        public override bool Equals(object? obj)
        {
            return obj is ReservarPistaDTO dTO &&
                   IdPista == dTO.IdPista &&
                   NombrePista == dTO.NombrePista &&
                   NPersonas == dTO.NPersonas &&
                   Precio == dTO.Precio &&
                   Stock == dTO.Stock   ;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdPista, NombrePista, NPersonas, Precio, Stock);
        }
    }
}