namespace AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs
{
    //dto con el resumen completo de una inscripcion a clase deportivas
   public class InscripcionClaseDeportivaDetailDTO
    {
        public InscripcionClaseDeportivaDetailDTO(int id, string? nombre, string? apellidos, string dni, string? email, string? telefono, DateTime fechaInscripcion, decimal precioTotal, MetodoPago metodoPago, IList<InscripcionClaseDeportivaItemDTO> clasesInscritas)
        {//constructor para crear el dto con los datos obtenids
            Id = id;
            Nombre = nombre;
            Apellidos = apellidos;
            DNI = dni;
            Email = email;
            Telefono = telefono;
            FechaInscripcion = fechaInscripcion;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            ClasesInscritas = clasesInscritas;//lista detallada de las clases de esta inscripcion
        }

        public int Id{get; set;}//identificador

        public string? Nombre{get; set;}//nombre, puede ser null

        public string? Apellidos{get; set;}//apellidos, puede ser null

        [Required]//obligatorio
        public string DNI{get; set;}//dni

        public string? Email{get; set;}//email , puede ser null

        public string? Telefono{get; set;}//telefono, puede ser null

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]//indicamos que es una fecha
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]//formato fecha
        public DateTime FechaInscripcion{get; set;}

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]// precio total = valor monetario
        [Range(0, 999.99)]//rango limite permitido
        public decimal PrecioTotal{get; set;}

        public MetodoPago MetodoPago{get; set;}//enum 

        public IList<InscripcionClaseDeportivaItemDTO> ClasesInscritas {get; set;}//Lista con cada una de las clases incluidas dentro de esa inscripcion

      
    }
}