using AppForSEII.API.DTOs.ReservarPistaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]//ruta base del controlador. [controller] se sustituye por el nombre de la clase sin la palabra Controller. seria: /api/PistaReservada
    [ApiController]//ndica que la clase es un controlador de una API web. ASP.NET Core. que trata errores de validación de parametros
    public class PistaReservadaController : ControllerBase// class PistaReservadaController: declara el controlador ; ControllerBase: funcionalidades para responder peticiones HTTP ej-> devolver un 200 OK o un 400 Bad Request.
    {
        private readonly ApplicationDbContext _context; //Declara una variable privada llamada _context, esta permite consultar las pistas almacenadas en la base de datos
        private readonly ILogger<PistaReservadaController> _logger;//Declara un objeto que permite guardar mensajes en registros o logs de la app.

        public PistaReservadaController(ApplicationDbContext context, ILogger<PistaReservadaController> logger)//Constructor de la clase PistaReservadaController.
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet] //para indicar que el metodo responde a las peticiones de HTTP GET
        [Route("[action]")]//indicar que la ruta de la acción se construye a partir del nombre del método. En este caso, la ruta sería /api/PistaReservada/GetPistasParaReservar
        [ProducesResponseType(typeof(IList<ReservarPistaDTO>), (int)HttpStatusCode.OK)]//cuando la operación tiene éxito, puede devolver un código HTTP 200 OK con lista de objetos ReservarPistaDTO.
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]//si hay error de validación, puede devolver un 400 Bad Request con información sobre el error en un objeto ValidationProblemDetails.
        public async Task<ActionResult<IList<ReservarPistaDTO>>> GetPistasParaReservar(string? nombrePista, int? nPersonas)//declara el método que busca las pistas. Task<...> representa una operación que terminará en el futuro. ActionResult<IList<ReservarPistaDTO>>: indica que la respuesta puede contener una lista de DTOs o un resultado HTTP, como un error.
        {
            if (nPersonas.HasValue && (nPersonas < 1 || nPersonas > 30)) //comprobar el numero de personas valido
            {
                ModelState.AddModelError("nPersonas", "El número de personas debe ser un valor positivo y como mucho 30.");//Añade un error de validación a ModelState, asociado al campo nPersonas
                _logger.LogError($"{DateTime.Now} Error: El número de personas debe ser un valor positivo y como mucho 30.");//Registra un mensaje de error.EL DateTime.Now da la fecha y hora actuales
                return BadRequest(new ValidationProblemDetails(ModelState));//Devuelve una respuesta HTTP 404 Bad Request
            }
        

        IList<ReservarPistaDTO> pistas = await _context.Set<Pista>() //IList<ReservarPistaDTO>declara la variable(pistas) que almacena la lista de DTOs. _context.Set<Pista>(): da el conjunto de entidades Pista que se consulta en la base de datos. await: espera a que termine la operación asíncrona.
            .Where(p=> p.Stock >0 && (nombrePista == null || p.NombrePista.Contains(nombrePista))// para buscar la pista . el nombrePista==null es para si no se pone nada, permite  cualquiera. EL p.NombrePista.Contains(nombrePista): es para si se pone nombre, comprueba que esté incluido en el nombre de la pista.
            && (nPersonas == null || p.NPersonas >= nPersonas)) //si no se indica el número de personas, permite cualquier capacidad. EL p.NPersonas >= nPersonas si  pone numero de personas, la capacidad debe ser suficiente
            .OrderBy(p=>p.NombrePista) //Ordena las pistas alfabéticamente por su nombre y ascendente
            .ThenBy(p=>p.Precio)//Si dos pistas tienen el mismo nombre, las ordena por el precio de menor a mayor
            .Select(p=> new ReservarPistaDTO(//Select() transforma cada entidad Pista en un nuevo objeto ReservarPistaDTO
                p.IdPista, //datos del constructor
                p.NombrePista,
                p.NPersonas,
                p.Precio,
                p.Stock))
                .ToListAsync();//Ejecuta la consulta de forma asíncrona y devuelve los resultados en una lista

        return Ok(pistas);//Devuelve una respuesta HTTP 200 OK, contiene la lista de pistas encontradas

        }
    }
}