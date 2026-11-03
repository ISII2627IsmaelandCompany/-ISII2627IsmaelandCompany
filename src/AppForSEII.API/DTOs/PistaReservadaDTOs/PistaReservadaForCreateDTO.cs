namespace AppForSEII.API.DTOs.PistaReservadaDTOs
{

    //DTO para recibir los datos de una nueva reserva de pista
    //Se usa para el Post para crear una reserva de pista
    public class PistaReservadaForCreateDTO
    {
        //constructor para crear el DTO pasando los datos
        public PistaReservadaForCreateDTO(
            string clienteUserName,//correo del cliente que hace la reserva
            MetodoPago metodoPago,//metodo de pago elegido
            IList<PistaReservadaItemDTO> pistasReservadas)//Lista de pistas seleccionados
        {//Guardamos el correo del cliente y si es null lanzamos error para que no se cree sin ese dato
            ClienteUserName = clienteUserName ?? throw new ArgumentNullException(nameof(clienteUserName));
            MetodoPago = metodoPago;//Guardamos el metodo de pago que ha elegido el cliente
            PistasReservadas = pistasReservadas ?? throw new ArgumentNullException(nameof(pistasReservadas));//Lista de pistas seleccionadas y mismo error que el de usuario
        }

         public PistaReservadaForCreateDTO()//constructor vacio para poder crear el DTO sin parametros
        {
            PistasReservadas = new List<PistaReservadaItemDTO>();//lista vacia de pistas reservadas
        }

        [EmailAddress]//comprobamos con esta etiqueta que el correo tenga un formato de email valido
        [Required]//obligatorio
        //correo del cliente que realiza la reserva
        //valor incial vacio obligar a introducir el valor
        public string ClienteUserName { get; set; } = string.Empty;


        [Required]//obligatorio
        //metodo de pago elegido por cliente (enum) y valor inicial tarjeta
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Tarjeta;
        //lista de pistas  que el cliente selecciona
        //ilist permite trabajar con una coleccion de ItemDTO
        //cada ItemDTO contiene los datos de una pista seleccionada
        public IList<PistaReservadaItemDTO> PistasReservadas { get; set; }
            = new List<PistaReservadaItemDTO>();//inicialmente lista vacia
            //calcula el precio total de la reserva
            //es de tipo decimal por el dinero
        public decimal PrecioTotal
        {//solo tiene get porque el precio se calcula automaticamente
            get
            {//sum suma el precio de cada pista reservada
            //pr representa cada ItemDTO de la lista
            //pr.Precio obtiene el precio de ese item 
            //devuelve la suma de todos los precios
                return (decimal)PistasReservadas.Sum(pr => pr.Precio); //el Precio del item es double, asi que convierto a decimal para que coincida con el PrecioTotal del Details
            }
        }
    }
}
