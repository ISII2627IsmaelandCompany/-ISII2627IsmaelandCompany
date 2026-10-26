using AppForSEII.API.DTOs.PistaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]//ruta base del controlador. [controller] se sustituye por el nombre de la clase sin la palabra Controller. aqui seria: /api/Pista
    [ApiController]//indica que esta clase es un controlador de una API web. ASP.NET Core. que trata errores de validación de parametros
    public class PistasController : ControllerBase// class PistaController: declara el controlador; ControllerBase: funcionalidad para responder peticiones HTTP ej-> devolver un 200 OK o un 400 Bad Request.
    {
        private readonly ApplicationDbContext _context; //Declara una variable privada llamada _context, que permite consultar las pistas almacenadas en la base de datos
        private readonly ILogger<PistasController> _logger;//Declara un objeto que permite guardar mensajes en registros o logs de la app.

        public PistasController(ApplicationDbContext context, ILogger<PistasController> logger)//Constructor del controlador PistaController, este recibe el contexto de la base de datos y el logger
        {
            _context = context;//guarda la base de datos en esta variable para usarla en los metodos
            _logger = logger;//guarda la informacion
        }

        [HttpGet] //para indicar que el metodo responde a las peticiones de HTTP GET
        [Route("[action]")]//indicar que la ruta de la acción se construye a partir del nombre del método. En este caso, la ruta sería /api/Pista/GetPistas
        [ProducesResponseType(typeof(IList<PistaDTO>), (int)HttpStatusCode.OK)]//cuando la operación tiene éxito, puede devolver un código HTTP 200 OK con lista de objetos PistaDTO
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]//si hay error de validación, puede devolver un 400 Bad Request con info del error
        public async Task<ActionResult<IList<PistaDTO>>> GetPistas(string? tipoDeporte, DateTime from, DateTime to)//declara el método que busca las pistas. Task<...> es una operación que terminará en el futuro. ActionResult<IList<PistaDTO>>: indica que la respuesta puede contener una lista de DTOs o un resultado HTTP, ej: un error
        {
            if (from != null && to != null && from > to) //comprueba que se hayan introducido 2 fechas y que from sea anterior que to
            {

                ModelState.AddModelError("from&to", "from must be earlier than to");
                _logger.LogError($"{DateTime.Now} Error: from must be earlier than to");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            //Si no se asignan fechas, se asignan valores ppor defecto
            from = from == null ? DateTime.Today.AddDays(1) : from;
            to = to == null ? DateTime.Today.AddDays(2) : to;

        IList<PistaDTO> pistas = await _context.Set<Pista>() //IList<PistaDTO>declara la variable(pistas) que almacena la lista de DTOs. _context.Set<Pista>(): da el conjunto de entidades Pista que se consulta en la base de datos. await: espera a que termine la operación asíncrona.
            .Include(p=>p.TipoDeporte)  //para sacar el tipo de deporte
            .Include(p => p.PistasReservadas)//Join con las líneas de reserva como en el ejemplo de movies
            .ThenInclude(pr => pr.Reserva)//join con la reserva
            .Where(p=> p.Stock >0 && (tipoDeporte == null || p.TipoDeporte.Nombre.Equals(tipoDeporte))// para buscar la pista . el tipoDeporte == null es para si no se pone nada, permite  cualquiera. EL p.TipoDeporte.Nombre.Equals(TipoDeporte): es para si se pone nombre, comprueba que esté incluido el nombre del deporte.
            && (p.PistasReservadas.Count(pr => pr.Reserva.FechaReserva.Date >= from && pr.Reserva.FechaReserva <= to) < p.Stock))//comprueba la disponibilidad de las pistas este dia
            .OrderBy(p=>p.NombrePista) //Ordena las pistas alfabéticamente por su nombre y ascendente
            .ThenBy(p=>p.Precio)//Si dos pistas tienen el mismo nombre, las ordena por el precio de menor a mayor
            .Select(p=> new PistaDTO(//Select() transforma cada entidad Pista en un nuevo objeto PistaDTO
                p.IdPista, //datos del constructor
                p.NombrePista,
                p.NPersonas,
                p.Precio,
                p.Stock,
                p.TipoDeporte.Nombre))//para pasar el noombre del tipoDeporte
                .ToListAsync();//Ejecuta la consulta de forma asíncrona y devuelve los resultados en una lista

        return Ok(pistas);//Devuelve una respuesta HTTP 200 OK, contiene la lista de pistas encontradas

        }
    }
}