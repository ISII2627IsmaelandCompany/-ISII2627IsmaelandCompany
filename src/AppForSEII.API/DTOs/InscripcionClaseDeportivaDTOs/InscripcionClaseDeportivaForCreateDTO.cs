namespace AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs
{
    public class InscripcionClaseDeportivaForCreateDTO
    {
        public InscripcionClaseDeportivaForCreateDTO(
            string clienteUserName,
            string datosPago,
            MetodoPago metodoPago,
            IList<InscripcionClaseDeportivaItemDTO> clasesInscritas)
        {
            ClienteUserName = clienteUserName ?? throw new ArgumentNullException(nameof(clienteUserName));
            DatosPago = datosPago ?? throw new ArgumentNullException(nameof(datosPago));
            MetodoPago = metodoPago;
            ClasesInscritas = clasesInscritas ?? throw new ArgumentNullException(nameof(clasesInscritas));
        }

        public InscripcionClaseDeportivaForCreateDTO()
        {
            ClasesInscritas = new List<InscripcionClaseDeportivaItemDTO>();
        }

        [EmailAddress]
        [Required]
        public string ClienteUserName { get; set; } = "peter@uclm.es";

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe introducir los datos de pago")]
        public string DatosPago { get; set; } = "Tarjeta terminada en 1234";

        [Required]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Tarjeta;

        public IList<InscripcionClaseDeportivaItemDTO> ClasesInscritas { get; set; }
            = new List<InscripcionClaseDeportivaItemDTO>();

        public decimal PrecioTotal
        {
            get
            {
                return ClasesInscritas.Sum(ci => ci.Precio);
            }
        }
    }
}