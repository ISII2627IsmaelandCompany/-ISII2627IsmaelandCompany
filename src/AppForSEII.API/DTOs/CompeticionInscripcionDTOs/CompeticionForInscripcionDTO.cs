
using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.InscripcionDTOs
{
    public class CompeticionForInscripcionDTO//DTO para representar la información de una competición que se utilizará en la inscripción
    {
        public CompeticionForInscripcionDTO()
        {
        }

        public CompeticionForInscripcionDTO(int id, string nombre, string lugar, DateTime fecha, int plazas, decimal precio)
        {
            Id = id;
            Nombre = nombre;
            Lugar = lugar;
            Fecha = fecha;
            Plazas = plazas;
            Precio = precio;
        }//constructor que inicializa las propiedades de la clase con los valores proporcionados

        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
        public string Nombre { get; set; } = "Competición";

        [StringLength(100, ErrorMessage = "El lugar no puede superar los 100 caracteres")]
        public string Lugar { get; set; } = "Lugar";

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]//para que se muestre solo la fecha en la vista
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]//para que se muestre solo la fecha en la vista
        [Required]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Las plazas no pueden ser negativas")]//para que se muestre solo la fecha en la vista
        public int Plazas { get; set; } = 0;//para que se muestre solo la fecha en la vista, las plazas no pueden ser negativas

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]//para que se muestre el símbolo de moneda en la vista
        [Range(typeof(decimal), "0", "9999999999.99", ErrorMessage = "El precio no puede ser negativo")]
        public decimal Precio { get; set; } = 0;//para que se muestre el símbolo de moneda en la vista, el precio no puede ser negativo

        public override bool Equals(object? obj)//comprueba si dos objetos son iguales
        {
            return obj is CompeticionForInscripcionDTO dto &&//comprueba si el objeto es del mismo tipo y si sus propiedades son iguales
                   Id == dto.Id &&
                   Nombre == dto.Nombre &&
                   Lugar == dto.Lugar &&
                   Fecha == dto.Fecha &&
                   Plazas == dto.Plazas &&
                   Precio == dto.Precio;
        }

        public override int GetHashCode()//devuelve un código hash para el objeto, que se utiliza para compararlo con otros objetos
        {
            return HashCode.Combine(
                Id, Nombre, Lugar, Fecha, Plazas, Precio);
        }
    }
}
