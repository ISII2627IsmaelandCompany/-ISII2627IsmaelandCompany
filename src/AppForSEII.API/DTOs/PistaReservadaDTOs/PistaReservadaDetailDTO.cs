namespace AppForSEII.API.DTOs.PistaReservadaDTOs
{
    public class PistaReservadaDetailDTO
    {
        public PistaReservadaDetailDTO(int id, DateTime fechaReserva, string? nombreCliente, string? apellidos, string dni, decimal precioTotal, MetodoPago metodoPago, IList<PistaReservadaItemDTO> pistasReservadas)
        {
            Id = id;
            FechaReserva = fechaReserva;
            NombreCliente = nombreCliente;
            Apellidos = apellidos;
            DNI = dni;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            PistasReservadas = pistasReservadas; //lista detallada de las pistas reservadas
        }

        public int Id { get; set; } //identificador

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
        public IList<PistaReservadaItemDTO> PistasReservadas { get; set; }////Lista con cada una de las clases incluidas dentro de esa reserva
    }
}