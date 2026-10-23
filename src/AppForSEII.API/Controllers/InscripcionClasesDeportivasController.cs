using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs;//Importar dto del details de inscripcion clase deportiva
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//
namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]//Ruta del controlador
    [ApiController]//controlador de la api
    public class InscripcionClasesDeportivasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;//para acceder a las tablas de la bbdd
        private readonly ILogger<InscripcionClasesDeportivasController> _logger;//registrar informacion o errores del controller

        public InscripcionClasesDeportivasController(ApplicationDbContext context,ILogger<InscripcionClasesDeportivasController> logger)
        {//constructor controller para proporcionar contexto e informacion
            _context = context;
            _logger = logger;
        }
        [HttpGet]//peticion HTTP GET
        [Route("[action]")]//nombre del metodo como parte de la  URL
        [ProducesResponseType(typeof(InscripcionClaseDeportivaDetailDTO), (int)HttpStatusCode.OK)]//todo correcto devuelve 200 OK e informacion
        [ProducesResponseType((int)HttpStatusCode.NotFound)]//va mal = 404 not found
        public async Task<ActionResult> GetInscripcion(int id)//id de la inscripcion a consultar la recibe por parametro
        {
            var inscripcion=await _context.Inscripciones.Where(i => i.Id == id)//filtra hasta encontrar esa id
    .Include(i => i.Cliente)//cargamos relacion entre inscripcion y aplicationuser
    .Include(i => i.ClasesInscritas)//relacion Clase inscrita con la inscripcion
        .ThenInclude(ci => ci.ClaseDeportiva)//para cada clase inscrita cargamos su clase deportiva
            .ThenInclude(cd => cd.TipoDeporte)//para cada clase deportiva cargamos su tipo deporte
            //dto que queremos devolver al cliente
    .Select(i => new InscripcionClaseDeportivaDetailDTO(i.Id,i.Cliente.Name,i.Cliente.Surname,i.Cliente.DNI,i.Cliente.Email,i.Cliente.PhoneNumber,i.FechaInscripcion,i.PrecioTotal,i.MetodoPago,i.ClasesInscritas
            .Select(ci => new InscripcionClaseDeportivaItemDTO(ci.ClaseDeportiva.Descripcion,ci.ClaseDeportiva.TipoDeporte.Nombre,ci.ClaseDeportiva.FechaHora,ci.PlazasReservadas,ci.Precio,ci.Observaciones)).ToList<InscripcionClaseDeportivaItemDTO>())).FirstOrDefaultAsync();
            //el id , datos del cliente , datos inscripcion , datos clase inscrita , datos clase deportiva. todo esto se devuelve en lista
            //entonces al final el firtordefault devuelve o null o toda la lista de inscripcion transformado a dto
             if (inscripcion == null)//si es null entra y mensaje de error mas el 404
    {
        _logger.LogError($"No se ha encontrado la inscripción con id {id}");
        return NotFound();
    }

    return Ok(inscripcion);//sino 200 e informacion inscripcion
        }
    }
}