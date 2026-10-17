using AppForSEII.API.DTOs.MaterialAlquiladoDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialesAlquiladosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MaterialesAlquiladosController> _logger;

        public MaterialesAlquiladosController(ApplicationDbContext context, ILogger<MaterialesAlquiladosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtiene la lista de materiales alquilados filtrados por ID de alquiler o ID de material.
        [HttpGet] //Peticion GET
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<MaterialAlquiladoDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<MaterialAlquiladoDTO>>> GetMaterialesAlquilados(int? idAlquiler, int? idMaterial)
        {
            // Validación opcional: Verificar parámetros no válidos
            if (idAlquiler.HasValue && idAlquiler.Value <= 0)
            {
                ModelState.AddModelError("idAlquiler", "El ID del alquiler debe ser mayor a 0.");
                _logger.LogError($"{DateTime.Now} Error: idAlquiler inválido ({idAlquiler}).");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            if (idMaterial.HasValue && idMaterial.Value <= 0)
            {
                ModelState.AddModelError("idMaterial", "El ID del material debe ser mayor a 0.");
                _logger.LogError($"{DateTime.Now} Error: idMaterial inválido ({idMaterial}).");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Consulta a la base de datos
            IList<MaterialAlquiladoDTO> listaMateriales = await _context.Set<MaterialAlquilado>()
                .Where(m => (idAlquiler == null || m.IdAlquiler == idAlquiler)
                         && (idMaterial == null || m.IdMaterial == idMaterial))
                .OrderBy(m => m.IdAlquiler)
                .ThenBy(m => m.IdMaterial)
                .Select(m => new MaterialAlquiladoDTO(
                    m.Cantidad,
                    m.Descripcion,
                    m.IdMaterial,
                    m.IdAlquiler,
                    m.Precio
                ))
                .ToListAsync();

            return Ok(listaMateriales);
        }
    }
}