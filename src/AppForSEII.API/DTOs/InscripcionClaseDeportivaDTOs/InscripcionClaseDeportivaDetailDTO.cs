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

        public override bool Equals(object? obj)//comparamos dos dtos de details para ver si son iguales para las pruebas unitarias
        {//comprobamos todas las propiedades de ambas inscripciones
            return obj is InscripcionClaseDeportivaDetailDTO dTO &&
                   Id == dTO.Id &&
                   Nombre == dTO.Nombre &&
                   Apellidos == dTO.Apellidos &&
                   DNI == dTO.DNI &&
                   Email == dTO.Email &&
                   Telefono == dTO.Telefono &&
                   FechaInscripcion == dTO.FechaInscripcion &&
                   PrecioTotal == dTO.PrecioTotal &&
                   MetodoPago == dTO.MetodoPago &&
                  ClasesInscritas.SequenceEqual(dTO.ClasesInscritas);//comparamos que ambas listas tengan la misma clases inscritas,en el mismo orden (compara elemento a elemento)
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Nombre);
            hash.Add(Apellidos);
            hash.Add(DNI);
            hash.Add(Email);
            hash.Add(Telefono);
            hash.Add(FechaInscripcion);
            hash.Add(PrecioTotal);
            hash.Add(MetodoPago);
            // añadimos cada clase inscrita para tener en cuenta el contenido de la lista
            foreach (var clase in ClasesInscritas)
         {
            hash.Add(clase);
        }            
            return hash.ToHashCode();
        }
    }
}