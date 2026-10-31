namespace AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs
{

    //DTO para recibir los datos de una nueva inscripcion
    //Se usa para el Post para crear una inscripcion a clases deportivas
    public class InscripcionClaseDeportivaForCreateDTO
    {
        //constructor para crear el DTO pasando los datos
        public InscripcionClaseDeportivaForCreateDTO(
            string clienteUserName,//correo del cliente
            string datosPago,//informacion sobre el pago
            MetodoPago metodoPago,//metodo de pago elegido
            IList<InscripcionClaseDeportivaItemDTO> clasesInscritas)//Lista de clases seleccionados
        {//Guardamos el correo del cliente y si es null lanzamos error para que no se cree sin ese dato
            ClienteUserName = clienteUserName ?? throw new ArgumentNullException(nameof(clienteUserName));
            DatosPago = datosPago ?? throw new ArgumentNullException(nameof(datosPago));//guardamos datos de pago y al igual que lo del correo con el null lo hacemos aqui tambien
            MetodoPago = metodoPago;//Guardamos el metodo de pago que ha elegido el cliente
            ClasesInscritas = clasesInscritas ?? throw new ArgumentNullException(nameof(clasesInscritas));//Lista de clases deportivas seleccionadas y mismo error que el de usuario
        }

        public InscripcionClaseDeportivaForCreateDTO()//constructor vacio para poder crear el DTO sin parametros
        {
            ClasesInscritas = new List<InscripcionClaseDeportivaItemDTO>();//lista vacia de clases inscritas
        }

        [EmailAddress]//comprobamos con esta etiqueta que el correo tenga un formato de email valido
        [Required]//obligatorio
        //correo del cliente que realiza la inscripcion
        //valor incial vacio obligar a introducir el valor
        public string ClienteUserName { get; set; } = string.Empty;
        //Los datos de pago son obligatorios
         // AllowEmptyStrings=false no permite cadenas vacias
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe introducir los datos de pago")]
        //informacion del pago introducida por el cliente
        //valor incial vacio obligar a introducir el valor
        public string DatosPago { get; set; } = string.Empty;

        [Required]//obligatorio
        //metodo de pago elegido por cliente (enum) y valor inicial tarjeta
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Tarjeta;
        //lista de clases deportivas que el cliente selecciona
        //ilist permite trabajar con una coleccion de ItemDTO
        //cada ItemDTO contiene los datos de una clase seleccionada
        public IList<InscripcionClaseDeportivaItemDTO> ClasesInscritas { get; set; }
            = new List<InscripcionClaseDeportivaItemDTO>();//inicialmente lista vacia
            //calcula el precio total de la inscripcion
            //es de tipo decimal por el dinero
        public decimal PrecioTotal
        {//solo tiene get porque el precio se calcula automaticamente
            get
            {//sum suma el precio de cada clase inscrita
            //ci representa cada ItemDTO de la lista
            //ci.Precio obtiene el precio de ese item 
            //devuelve la suma de todos los precios
                return ClasesInscritas.Sum(ci => ci.Precio);
            }
        }
    }
}