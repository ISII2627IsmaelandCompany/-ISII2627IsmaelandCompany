using AppForSEII.API.DTOs.MaterialAlquiladoDTOs; // DTOs del details de alquiler
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")] // ruta del controlador
    [ApiController] // controlador de la api
    public class AlquileresController : ControllerBase  // controlador sin vistas, solo para devolver datos
    {
        private readonly ApplicationDbContext _context; // acceso a las tablas de la bbdd
        private readonly ILogger<AlquileresController> _logger; // registrar información o errores

        // constructor inyecta automáticamente el DbContext y el logger
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
            .Include(a => a.MaterialesAlquilados)  // carga la lista de materiales alquilados de este alquiler
                .ThenInclude(ma => ma.Material)   // y, de cada uno, el Material al que se refiere
                    .ThenInclude(m => m.TipoMaterial)  // y, de cada Material, su TipoMaterial (balones o raquetas)
            .Include(a => a.MaterialesAlquilados)  // se repite el Include para abrir otra rama del árbol de relaciones
                .ThenInclude(ma => ma.Material)
                    .ThenInclude(m => m.TipoDeporte)  // y, de cada Material, su TipoDeporte (baloncesto o tenis)
            // Select proyecta la entidad en un DTO, así solo enviamos los datos necesarios
            .Select(a => new MaterialAlquiladoDetailDTOs(
                a.IdAlquiler,
                a.Cliente.Name,
                a.Cliente.Surname,
                a.Cliente.DNI,
                a.Cliente.Email,
                a.Cliente.PhoneNumber,
                a.FechaAlquiler,
                a.PrecioTotal,
                a.MetodoPago,
                a.MaterialesAlquilados
                    .Select(ma => new MaterialAlquiladoItemDTOs(  // cada línea se convierte en un DTO "item"
                        ma.Material.Nombre,
                        ma.Material.TipoMaterial.NombreTipoMaterial,
                        ma.Material.TipoDeporte.Nombre,
                        ma.Cantidad,
                        ma.Precio))
                    .ToList<MaterialAlquiladoItemDTOs>()))
            .FirstOrDefaultAsync(); // devuelve el primero o null si no hay ninguno consulta la bbdd

            if (alquiler == null) // si es null error 404
            {
                _logger.LogError($"No se ha encontrado el alquiler con id {id}"); // dejamos constancia en el log
                return NotFound(); // 404 Not Found
            }

            return Ok(alquiler); // 200 + detalle del alquiler
        }
    }
}