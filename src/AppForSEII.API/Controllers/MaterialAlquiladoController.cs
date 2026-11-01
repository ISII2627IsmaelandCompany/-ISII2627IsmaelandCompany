using AppForSEII.API.DTOs.MaterialAlquiladoDTOs; // DTOs del details de alquiler
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")] // Ruta del controlador
    [ApiController] // Controlador de la api
    public class AlquileresController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // acceso a las tablas de la bbdd
        private readonly ILogger<AlquileresController> _logger; // registrar información o errores

        public AlquileresController(ApplicationDbContext context, ILogger<AlquileresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet] // Petición HTTP GET
        [Route("[action]")] // Nombre del método como parte de la URL
        [ProducesResponseType(typeof(MaterialAlquiladoDetailDTOs), (int)HttpStatusCode.OK)] // 200 OK + información
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // 404 Not Found
        public async Task<ActionResult> GetAlquiler(int id) // id del alquiler a consultar
        {
            var alquiler = await _context.Alquileres
    .Where(a => a.IdAlquiler == id) // filtra por el id del alquiler
    .Include(a => a.Cliente) // relación alquiler - ApplicationUser
    .Include(a => a.MaterialesAlquilados)
        .ThenInclude(ma => ma.Material)
            .ThenInclude(m => m.TipoMaterial)
    .Include(a => a.MaterialesAlquilados)
        .ThenInclude(ma => ma.Material)
            .ThenInclude(m => m.TipoDeporte)
    .Select(a => new MaterialAlquiladoDetailDTOs(
        a.IdAlquiler,
        a.Cliente.Name,
        a.Cliente.Surname,
        a.Cliente.DNI,
        a.Cliente.Email,
        a.Cliente.PhoneNumber, // en ApplicationUser el teléfono es PhoneNumber, no NumeroTelfono
        a.FechaAlquiler,
        a.PrecioTotal,
        a.MetodoPago,
        a.MaterialesAlquilados
            .Select(ma => new MaterialAlquiladoItemDTOs(
                ma.Material.Nombre,
                ma.Material.TipoMaterial.NombreTipoMaterial,
                ma.Material.TipoDeporte.Nombre,
                ma.Cantidad,
                ma.Precio))
            .ToList<MaterialAlquiladoItemDTOs>()))
    .FirstOrDefaultAsync();

            if (alquiler == null) // si es null: error + 404
            {
                _logger.LogError($"No se ha encontrado el alquiler con id {id}");
                return NotFound();
            }

            return Ok(alquiler); // 200 + detalle del alquiler
        }
    }
}