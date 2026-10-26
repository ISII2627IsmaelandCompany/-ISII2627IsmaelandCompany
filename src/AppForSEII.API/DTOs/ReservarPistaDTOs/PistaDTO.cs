using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace AppForSEII.API.DTOs.PistaDTOs
{
    public class PistaDTO
    {
        public PistaDTO()
        {
        }

        public PistaDTO(int idPista, string nombrePista, int nPersonas, double precio, int stock, string tipoDeporte)
        {
            IdPista = idPista;
            NombrePista = nombrePista;
            NPersonas = nPersonas;
            Precio = precio;
            Stock = stock;
            TipoDeporte = tipoDeporte; 
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
        
        [Required]
        [StringLength(50, ErrorMessage = "El tipo de deporte no puede tener mas de 50 caracteres")]
        public string TipoDeporte{ get; set; }= string.Empty; 
        public override bool Equals(object? obj)
        {
            return obj is PistaDTO dTO &&
                   IdPista == dTO.IdPista &&
                   NombrePista == dTO.NombrePista &&
                   NPersonas == dTO.NPersonas &&
                   Precio == dTO.Precio &&
                   Stock == dTO.Stock &&
                   TipoDeporte == dTO.TipoDeporte;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdPista, NombrePista, NPersonas, Precio, Stock, TipoDeporte);
        }
    }
}