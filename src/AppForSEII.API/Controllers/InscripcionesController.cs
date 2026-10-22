using AppForSEII.API.DTOs.InscripcionDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InscripcionesController> _logger;

        public InscripcionesController(ApplicationDbContext context,ILogger<InscripcionesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(InscripcionDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetInscripcion(int id)
        {
            var inscripcion=await _context.Inscripciones.Where(i => i.Id == id)
    .Include(i => i.Cliente)
    .Include(i => i.ClasesInscritas)
        .ThenInclude(ci => ci.ClaseDeportiva)
            .ThenInclude(cd => cd.TipoDeporte)
    .Select(i => new InscripcionDetailDTO(i.Id,i.Cliente.Name,i.Cliente.Surname,i.Cliente.DNI,i.Cliente.Email,i.Cliente.PhoneNumber,i.FechaInscripcion,i.PrecioTotal,i.MetodoPago,i.ClasesInscritas
            .Select(ci => new InscripcionItemDTO(ci.ClaseDeportiva.Descripcion,ci.ClaseDeportiva.TipoDeporte.Nombre,ci.ClaseDeportiva.FechaHora,ci.PlazasReservadas,ci.Precio,ci.Observaciones)).ToList<InscripcionItemDTO>())).FirstOrDefaultAsync();
             if (inscripcion == null)
    {
        _logger.LogError($"No se ha encontrado la inscripción con id {id}");
        return NotFound();
    }

    return Ok(inscripcion);
        }
    }
}