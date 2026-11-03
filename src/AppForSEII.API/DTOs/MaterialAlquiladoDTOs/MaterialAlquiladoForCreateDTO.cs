using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.MaterialAlquiladoDTOs
{
    // Cada línea que envía el cliente: qué material y cuántas unidades
    public class MaterialAlquiladoItemForCreateDTO
    {
        public MaterialAlquiladoItemForCreateDTO() { }

        public MaterialAlquiladoItemForCreateDTO(int materialId, int cantidad)
        {
            MaterialId = materialId;
            Cantidad = cantidad;
        }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        // Dos items son iguales si apuntan al mismo material y misma cantidad
        public override bool Equals(object? obj)
        {
            return obj is MaterialAlquiladoItemForCreateDTO other &&
                   MaterialId == other.MaterialId &&
                   Cantidad == other.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MaterialId, Cantidad);
        }
    }

    // DTO que recibe el POST
    public class MaterialAlquiladoForCreateDTO
    {
        public MaterialAlquiladoForCreateDTO() { }

        public MaterialAlquiladoForCreateDTO(string clienteUserName, string dni, string nombre,
            string apellidos, string telefono, DateTime fechaAlquiler, MetodoPago metodoPago,
            string? descripcion, IList<MaterialAlquiladoItemForCreateDTO> materiales)
        {
            ClienteUserName = clienteUserName ?? throw new ArgumentNullException(nameof(clienteUserName));
            DNI = dni ?? throw new ArgumentNullException(nameof(dni));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellidos = apellidos ?? throw new ArgumentNullException(nameof(apellidos));
            Telefono = telefono ?? throw new ArgumentNullException(nameof(telefono));
            FechaAlquiler = fechaAlquiler;
            MetodoPago = metodoPago;
            Descripcion = descripcion;
            Materiales = materiales ?? throw new ArgumentNullException(nameof(materiales));
        }

        [EmailAddress]
        [Required]
        public string ClienteUserName { get; set; } = string.Empty; // correo del cliente

        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string DNI { get; set; } = string.Empty;                 //DNI

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;              //nombre

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string Apellidos { get; set; } = string.Empty;           //apellidos

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = string.Empty;     //numero de telefono

        [Required(ErrorMessage = "Debe introducir la fecha del alquiler")]
        public DateTime FechaAlquiler { get; set; }        //fecha del alquiler

        [Required]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Tarjeta;

        [StringLength(500)]
        public string? Descripcion { get; set; } // opcional: qué va a hacer con el material

        public IList<MaterialAlquiladoItemForCreateDTO> Materiales { get; set; }
            = new List<MaterialAlquiladoItemForCreateDTO>();

        // Igualdad por valor: compara todos los campos y la lista de materiales (sin importar el orden)
        public override bool Equals(object? obj)
        {
            return obj is MaterialAlquiladoForCreateDTO other &&
                   ClienteUserName == other.ClienteUserName &&
                   DNI == other.DNI &&
                   Nombre == other.Nombre &&
                   Apellidos == other.Apellidos &&
                   Telefono == other.Telefono &&
                   FechaAlquiler == other.FechaAlquiler &&
                   MetodoPago == other.MetodoPago &&
                   Descripcion == other.Descripcion &&
                   Materiales.Count == other.Materiales.Count &&
                   !Materiales.Except(other.Materiales).Any();
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(ClienteUserName);
            hash.Add(DNI);
            hash.Add(Nombre);
            hash.Add(Apellidos);
            hash.Add(Telefono);
            hash.Add(FechaAlquiler);
            hash.Add(MetodoPago);
            hash.Add(Descripcion);

            // Suma de hashes de los materiales: el resultado no depende del orden
            // (coherente con el Equals, que tampoco tiene en cuenta el orden)
            int materialesHash = 0;
            foreach (var m in Materiales)
                materialesHash += m.GetHashCode();
            hash.Add(materialesHash);

            return hash.ToHashCode();
        }
    }
}