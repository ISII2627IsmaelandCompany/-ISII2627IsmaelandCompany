namespace AppForSEII.API.DTOs.InscripcionDTOs
{
    public class InscripcionItemDTO
    {
        public InscripcionItemDTO(string descripcion, string tipoDeporte, DateTime fechaHora, int plazasReservadas, decimal precio, string? observaciones)
        {
            Descripcion = descripcion;
            TipoDeporte = tipoDeporte;
            FechaHora = fechaHora;
            PlazasReservadas = plazasReservadas;
            Precio = precio;
            Observaciones = observaciones;
        }

        [Required]
        public string Descripcion{get;set;}
        [Required]
        public string TipoDeporte{get;set;}
        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime FechaHora{get;set;}
        [Range(1,3)]
        public int PlazasReservadas{get;set;}
        
         [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
         [Range(0,999.99)]
        public decimal Precio{get;set;}
        public string? Observaciones{get;set;}
        
        




    }



}