using AppForSEII.API.Data;
using AppForSEII.API.DTOs.InscripcionDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.EntityFrameworkCore;


namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]//ruta base del controlador
    [ApiController]
    public class InscripcionesController : ControllerBase//controlador para manejar las inscripciones a competiciones
    {
        private readonly ApplicationDbContext _context;//contexto de la base de datos para acceder a los datos de la aplicación
        private readonly ILogger<InscripcionesController> _logger;//log para registrar información y errores en el controlador

        public InscripcionesController(ApplicationDbContext context, ILogger<InscripcionesController> logger)//constructor del controlador
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]//acción para obtener las competiciones disponibles para la inscripción
        [Route("[action]")]//ruta de la acción
        [ProducesResponseType(typeof(IList<CompeticionForInscripcionDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]//tipos de respuesta que puede devolver la acción
        public async Task<ActionResult> GetCompeticionesForInscripcion(string? nombre, string? tipoDeporte, DateTime? fromDate, DateTime? toDate)//acción para obtener las competiciones disponibles para la inscripción, con filtros opcionales por nombre, tipo de deporte y rango de fechas
        {
            if (fromDate != null &&
                toDate != null &&
                fromDate > toDate)//comprueba si fromDate es posterior a toDate
            {
                ModelState.AddModelError(
                    "fromDate&toDate",
                    "fromDate debe ser anterior a toDate");

                _logger.LogError(
                    $"{DateTime.Now} Error: fromDate ({fromDate}) debe ser anterior a toDate ({toDate})");

                return BadRequest(new ValidationProblemDetails(ModelState));//devuelve un error 400 con los detalles del error si fromDate es posterior a toDate
            }
            

            var competiciones = await _context.Competiciones//obtiene las competiciones que cumplen con los criterios de filtro
                .Where(c =>
                    (nombre == null ||
                     c.Nombre.Contains(nombre)) &&

                    (tipoDeporte == null ||
                    (c.TipoDeporte != null && c.TipoDeporte.Nombre == tipoDeporte)) &&

                    (fromDate == null ||
                     c.Fecha >= fromDate.Value) &&

                    (toDate == null ||
                     c.Fecha <= toDate.Value) &&

                    c.CompeticionInscripciones.Count < c.Plazas)
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Precio)
                .Select(c => new CompeticionForInscripcionDTO(
                    c.Id,
                    c.Nombre,
                    c.Lugar,
                    c.Fecha,
                    c.Plazas,
                    c.Precio))
                .ToListAsync();//convierte la lista de competiciones a una lista de DTOs para la inscripción


            if(competiciones.Count == 0)//comprueba si no hay competiciones con plazas disponibles para la inscripción
            {
                ModelState.AddModelError(
                    "Plazas",
                    "No hay plazas disponibles para la competición");

                _logger.LogError(
                    $"{DateTime.Now} Error: No hay plazas disponibles para la competición");

                return BadRequest(new ValidationProblemDetails(ModelState));//devuelve un error 400 con los detalles del error si no hay plazas disponibles para la competición
            }
            
            return Ok(competiciones);//devuelve un código 200 con la lista de competiciones disponibles para la inscripción
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(InscripcionDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetInscripcion(int id)
        {
            var inscripcion = await _context.Inscripciones
                .Where(i => i.Id == id)
                .Select(i => new InscripcionDetailDTO(
                    i.Id,
                    i.Cliente.Name,
                    i.Cliente.Surname,
                    i.Cliente.DNI,
                    i.Cliente.Email,
                    i.Cliente.PhoneNumber,
                    i.FechaInscripcion,
                    i.PrecioTotal,
                    i.MetodoPago,
                    i.CompeticionInscripciones
                        .Select(ci => new InscripcionItemDTO(
                            ci.Competicion.Nombre,
                            ci.Competicion.Lugar,
                            ci.Competicion.Fecha,
                            ci.Competicion.Precio,
                            ci.ProblemasFisicos
                        ))
                        .ToList()
                ))
                .FirstOrDefaultAsync();

            if (inscripcion == null)
            {
                _logger.LogError($"Error: Inscripcion with id {id} does not exist");
                return NotFound();
            }

            return Ok(inscripcion);
        }
    }
}

