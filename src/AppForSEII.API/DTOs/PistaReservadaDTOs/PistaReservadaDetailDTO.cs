namespace AppForSEII.API.DTOs.PistaReservadaDTOs
{
    public class PistaReservadaDetailDTO
    {
        public PistaReservadaDetailDTO(int id, string? nombre,string? apellidos, string dni, string? email, string? telefono, DateTime fechaReserva ,decimal precioTotal, MetodoPago metodoPago, IList<PistaReservadaItemDTO> pistasReservadas)
        {
            Id = id;
            FechaReserva = fechaReserva;
            NombreCliente = nombre;
            Apellidos = apellidos;
            DNI = dni;
            Email = email;
            Telefono = telefono;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            PistasReservadas = pistasReservadas; //lista detallada de las pistas reservadas
        }

        public int Id { get; set; } //identificador
        public string? Email { get; set; } //email , puede ser null
        public string? Telefono { get; set; } //telefono, puede ser null

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]//indicamos que es una fecha
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]//formato fecha
        public DateTime FechaReserva { get; set; }
        
        public string? NombreCliente { get; set; } //nombre, puede ser null
        public string? Apellidos { get; set; } //apellidos, puede ser null
        public string DNI { get; set; } 

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]// precio total = valor monetario
        [Range(0, 999.99)]//rango limite permitido
        public decimal PrecioTotal { get; set; }
        public MetodoPago MetodoPago { get; set; }//es una enumeracion
        public IList<PistaReservadaItemDTO> PistasReservadas { get; set; }////Lista con cada una de las pistas incluidas dentro de esa reserva
    

    
        public override bool Equals(object? obj)//comparamos dos dtos de details para ver si son iguales para las pruebas unitarias
        {//comprobamos todas las propiedades de ambas inscripciones
            return obj is PistaReservadaDetailDTO dTO &&
                   Id == dTO.Id &&
                   NombreCliente == dTO.NombreCliente &&
                   Apellidos == dTO.Apellidos &&
                   DNI == dTO.DNI &&
                   Email == dTO.Email &&
                   Telefono == dTO.Telefono &&
                   FechaReserva == dTO.FechaReserva &&
                   PrecioTotal == dTO.PrecioTotal &&
                   MetodoPago == dTO.MetodoPago &&
                  PistasReservadas.SequenceEqual(dTO.PistasReservadas);//comparamos que ambas listas tengan la misma pistas reservadas ,en el mismo orden (compara elemento a elemento)
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(NombreCliente);
            hash.Add(Apellidos);
            hash.Add(DNI);
            hash.Add(Email);
            hash.Add(Telefono);
            hash.Add(FechaReserva);
            hash.Add(PrecioTotal);
            hash.Add(MetodoPago);
            // añadimos cada pista reservada para tener en cuenta el contenido de la lista
            foreach (var pista in PistasReservadas)
         {
            hash.Add(pista);
        }            
            return hash.ToHashCode();
        }
    
    
    
    
    
    
    }
}