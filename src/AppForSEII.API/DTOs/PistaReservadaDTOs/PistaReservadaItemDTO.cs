namespace AppForSEII.API.DTOs.PistaReservadaDTOs
{
    public class PistaReservadaItemDTO//dto de una pista reservada con el detalle de la reserva
    {                               //añadimos  IdPista para cuando se hace  el post(al crear una reserva) tener claro a que pista se refiere
        public PistaReservadaItemDTO(int idPista, string nombrePista, string tipoDeporte, string? observaciones, double precio, int cantidad)
        {//constructor para crear un item por cada pista reservada
           
            IdPista = idPista;
            NombrePista = nombrePista;
            TipoDeporte = tipoDeporte;
            Observaciones = observaciones;
            Precio = precio;
            Cantidad = cantidad;
        }

        public int IdPista { get; set; }//id de  la pista de la base de datos
        [Required]
        public string NombrePista { get; set; }
        [Required]
        public string TipoDeporte { get; set; }
        public string? Observaciones { get; set; }
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] 
         [Range(0,999.99)]//rango del precio
        public double Precio { get; set; }
        [Range(1, 30, ErrorMessage = "La cantidad debe estar entre 1 y 30.")]
        public int Cantidad { get; set; }
    

    public override bool Equals(object? obj)//comparamos dos ItemDto para las pruebas unitarias
        {//comparamos propiedades
            return obj is PistaReservadaItemDTO dTO &&
                   IdPista == dTO.IdPista &&
                   NombrePista == dTO.NombrePista &&
                   TipoDeporte == dTO.TipoDeporte &&
                   Observaciones == dTO.Observaciones &&
                   Precio == dTO.Precio &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
    // Si dos ItemDTO tienen los mismos datos, generaran el mismo hash
            return HashCode.Combine(IdPista, NombrePista, TipoDeporte, Observaciones, Precio, Cantidad);
        }
    }
}    