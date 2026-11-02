using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.MaterialAlquiladoDTOs
{
    // DTO es como un objeto que viaja de la API al cliente con los datos del details del alquiler.
    // Junta datos de varias entidades (Alquiler + Cliente + Materiales) en un solo objeto plano.
    public class MaterialAlquiladoDetailDTOs
    {
        // Constructor con todos los datos: es el que se llama en el Select del controlador
        public MaterialAlquiladoDetailDTOs(int idAlquiler, string? nombre, string? apellidos, string dni,
            string? email, string? telefono, DateTime fechaAlquiler, int precioTotal, MetodoPago metodoPago,
            IList<MaterialAlquiladoItemDTOs> materialesAlquilados) // string? = puede ser null, hace que sea opcional
        {
            IdAlquiler = idAlquiler;  // asignamos cada parámetro a su propiedad
            Nombre = nombre;
            Apellidos = apellidos;
            DNI = dni;
            Email = email;
            Telefono = telefono;
            FechaAlquiler = fechaAlquiler;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            MaterialesAlquilados = materialesAlquilados;
        }

        public int IdAlquiler { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }

        [Required] //obligatorio porque no puede ser vacio
        public string DNI { get; set; }

        public string? Email { get; set; }
        public string? Telefono { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAlquiler { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]  // indica que es dinero
        [Range(0, int.MaxValue)]  // el precio total no puede ser negativo
        public int PrecioTotal { get; set; }

        public MetodoPago MetodoPago { get; set; }

        public IList<MaterialAlquiladoItemDTOs> MaterialesAlquilados { get; set; } // lista de líneas del alquiler

        // Con Equals dos DTOs son iguales si TODOS sus campos coinciden (comparación por valor, no por referencia)
        public override bool Equals(object? obj)
        {
            return obj is MaterialAlquiladoDetailDTOs dto && // comprueba que obj es de este tipo y lo guarda en "dto"
                   IdAlquiler == dto.IdAlquiler &&
                   Nombre == dto.Nombre &&
                   Apellidos == dto.Apellidos &&
                   DNI == dto.DNI &&
                   Email == dto.Email &&
                   Telefono == dto.Telefono &&
                   FechaAlquiler == dto.FechaAlquiler &&
                   PrecioTotal == dto.PrecioTotal &&
                   MetodoPago == dto.MetodoPago &&
                   MaterialesAlquilados.SequenceEqual(dto.MaterialesAlquilados); // compara las listas elemento a elemento (usa el Equals del Item); el orden importa
        }

        // GetHashCode debe ser coherente con Equals (objetos iguales = mismo hash)
        public override int GetHashCode()
        {
            var hash = new HashCode(); // HashCode.Combine solo admite hasta 8 valores y aquí hay más, por eso se usa Add
            hash.Add(IdAlquiler);
            hash.Add(Nombre);
            hash.Add(Apellidos);
            hash.Add(DNI);
            hash.Add(Email);
            hash.Add(Telefono);
            hash.Add(FechaAlquiler);
            hash.Add(PrecioTotal);
            hash.Add(MetodoPago);
            foreach (var item in MaterialesAlquilados) // la lista no se puede añadir directamente (hashearía la referencia), así que añadimos cada elemento
            {
                hash.Add(item);
            }
            return hash.ToHashCode(); // devuelve el hash final combinado
        }
    }
}