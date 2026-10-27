namespace AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs
{//dto de una clase deportiva con el detalle de inscripcion
    public class InscripcionClaseDeportivaItemDTO
    {
        public InscripcionClaseDeportivaItemDTO(int claseDeportivaId,string descripcion, string tipoDeporte, DateTime fechaHora, int plazasReservadas, decimal precio, string? observaciones)
        {//constructor para crear un item por cada claseInscrita
        ClaseDeportivaId=claseDeportivaId;
            Descripcion = descripcion;
            TipoDeporte = tipoDeporte;
            FechaHora = fechaHora;
            PlazasReservadas = plazasReservadas;
            Precio = precio;
            Observaciones = observaciones;
        }
        public int ClaseDeportivaId { get; set; }
        [Required]//obligatorio
        public string Descripcion{get;set;}
        [Required]//obligatorio
        public string TipoDeporte{get;set;}//nombre deporte ej: futbol, baloncesto, tenis
        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]//contiene fecha y hora
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]//formato
        public DateTime FechaHora{get;set;}
        [Range(1,3)]//rango permitido segun el CU4 entre 1 y 3 plazas a reservar
        public int PlazasReservadas{get;set;}
        
         [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)] //indicamos que es dinero
         [Range(0,999.99)]//rango del precio
        public decimal Precio{get;set;}
        public string? Observaciones{get;set;}//observaciones, puede ser null 
        
        




    }



}