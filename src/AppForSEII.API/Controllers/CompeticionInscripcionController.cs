using AppForSEII.API.Data;
using AppForSEII.API.DTOs.InscripcionDTOs;
using Microsoft.AspNetCore.Mvc;


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
        [ProducesResponseType(typeof(IList<CompeticionForInscripcionDTO>), (int)HttpStatusCode.OK)][ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]//tipos de respuesta que puede devolver la acción
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

            return Ok(competiciones);//devuelve un código 200 con la lista de competiciones disponibles para la inscripción
        }
    }
}
