using AppForSEII.API.DTOs.PistaReservadaDTOs;//Importar dto del details de  pista reservada
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]//Ruta del controlador
    [ApiController]//controlador de la api

    public class PistaReservadaController : ControllerBase
    {

        private readonly ApplicationDbContext _context;//para acceder a las tablas de la bbdd
        private readonly ILogger<PistaReservadaController> _logger;//registrar informacion o errores del controller

        public PistaReservadaController(ApplicationDbContext context,ILogger<PistaReservadaController> logger)
        {//constructor controller para proporcionar contexto e informacion
            _context = context;
            _logger = logger;
        }

        [HttpGet]//peticion HTTP GET
        [Route("[action]")]//nombre del metodo como parte de la  URL
        [ProducesResponseType(typeof(PistaReservadaDetailDTO), (int)HttpStatusCode.OK)]//todo correcto devuelve 200 OK e informacion
        [ProducesResponseType((int)HttpStatusCode.NotFound)]//va mal = 404 not found

        public async Task<ActionResult> GetReserva(int id)//id de la reserva a consultar, la recibe por parametro
        {
            
            var reserva=await _context.Reservas.Where(i => i.Id == id)//filtra hasta encontrar esa id
    .Include(r => r.PistasReservadas)// Cargamos la lista de pistas reservadas de esta reserva
        .ThenInclude(pr => pr.Pista)// Para cada pista reservada, cargamos su Pista
            .ThenInclude(p => p.TipoDeporte)//para cada pista cargamos su tipo deporte
            //dto que queremos devolver al cliente
    .Select(r => new PistaReservadaDetailDTO(r.Id,r.FechaReserva,r.NombreCliente,r.Apellidos,r.DNI,r.PrecioTotal,r.MetodoPago,r.PistasReservadas
            .Select(pr => new PistaReservadaItemDTO(pr.IdPista,pr.Pista.NombrePista,pr.Pista.TipoDeporte.Nombre,pr.Observaciones,pr.Precio,pr.Cantidad)).ToList<PistaReservadaItemDTO>())).FirstOrDefaultAsync();
            //el id , datos del cliente , datos inscripcion , datos clase inscrita , datos clase deportiva. todo esto se devuelve en lista
            //entonces al final el firtordefault devuelve o null o toda la lista de inscripcion transformado a dto
             if (reserva == null)//si es null entra y mensaje de error mas el 404
    {
        _logger.LogError($"No se ha encontrado la reserva con id {id}");
        return NotFound();
    }

    return Ok(reserva);//sino 200 e informacion reserva
        }
    }   
}
