using AppForSEII.API.DTOs.MaterialDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MaterialesController> _logger;

        public MaterialesController(ApplicationDbContext context, ILogger<MaterialesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtiene la lista de materiales para alquilar, aplicando filtros opcionales (ID, nombre, precio máximo y tipo de material)
        [HttpGet] // Petición GET
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<MaterialDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<MaterialDTO>>> GetMateriales(int? idMaterial, string? nombre, decimal? precioMax, int? idTipoMaterial)
        {
            // Validaciones opcionales de parámetros
            if (idMaterial.HasValue && idMaterial.Value <= 0)
            {
                ModelState.AddModelError("idMaterial", "El ID del material debe ser mayor a 0.");
                _logger.LogError($"{DateTime.Now} Error: idMaterial inválido ({idMaterial}).");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            if (precioMax.HasValue && precioMax.Value < 0)
            {
                ModelState.AddModelError("precioMax", "El precio máximo no puede ser negativo.");
                _logger.LogError($"{DateTime.Now} Error: precioMax inválido ({precioMax}).");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            if (idTipoMaterial.HasValue && idTipoMaterial.Value <= 0)
            {
                ModelState.AddModelError("idTipoMaterial", "El ID del tipo de material debe ser mayor a 0.");
                _logger.LogError($"{DateTime.Now} Error: idTipoMaterial inválido ({idTipoMaterial}).");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Consulta a la base de datos aplicando todos los filtros de forma acumulativa (AND)
            IList<MaterialDTO> listaMateriales = await _context.Materials
                .Where(m => (!idMaterial.HasValue || m.IdMaterial == idMaterial.Value) &&
                            (string.IsNullOrEmpty(nombre) || m.Nombre.Contains(nombre)) &&
                            (!precioMax.HasValue || m.Precio <= precioMax.Value) &&
                            (!idTipoMaterial.HasValue || m.TipoMaterialId == idTipoMaterial.Value))
                .OrderBy(m => m.Nombre)
                .Select(m => new MaterialDTO(
                    m.IdMaterial,
                    m.Nombre,
                    m.Precio,
                    m.Cantidad
                ))
                .ToListAsync();

            return Ok(listaMateriales);
        }
    }
}