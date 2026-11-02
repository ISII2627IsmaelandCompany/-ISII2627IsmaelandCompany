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

        public async Task<ActionResult> GetPistaReservada(int idPista, int idReserva)//id de la reserva a consultar, la recibe por parametro
        {
            
            var Pistareservada=await _context.PistaReservada
            .Where(pr => pr.IdPista == idPista && pr.IdReserva == idReserva)//filtra hasta encontrar esa id
        .Include (pr => pr.Pista)// Cargamos el cliente de esta reserva
        .ThenInclude(p => p.TipoDeporte)// Para cada pista cargamos su tipo de deporte
            .Include(pr => pr.Reserva)//para cada pista reservada cargamos la reserva
            .ThenInclude(r => r.Cliente)//para cada reserva cargamos el cliente
        .Include(pr => pr.Reserva)
        .ThenInclude(r => r.PistasReservadas)//para cada reserva cargamos las pistas reservadas
        .ThenInclude(pRes => pRes.Pista)//para cada pista reservada cargamos la pista
        .ThenInclude(p => p.TipoDeporte)//para cada pista cargamos el tipo de deporte
            //dto que queremos devolver al cliente
    .Select(pr => new PistaReservadaDetailDTO(pr.Reserva.Id,pr.Reserva.Cliente.Name,pr.Reserva.Cliente.Surname,pr.Reserva.Cliente.DNI, pr.Reserva.Cliente.Email,pr.Reserva.Cliente.PhoneNumber,pr.Reserva.FechaReserva,pr.Reserva.PrecioTotal,pr.Reserva.MetodoPago,
    pr.Reserva.PistasReservadas.Select(pRes => new PistaReservadaItemDTO(pRes.IdPista,pRes.Pista.NombrePista,pRes.Pista.TipoDeporte.Nombre,pRes.Observaciones,pRes.Precio,pRes.Cantidad)).ToList())).FirstOrDefaultAsync();
          
    if (Pistareservada == null)            
    {
        _logger.LogError($"No se ha encontrado la pista reservada con  con id IdPista {idPista} e IdReserva {idReserva}");
        return NotFound();
    }

    return Ok(Pistareservada);//sino 200 e informacion reserva
        }
    }   
}
