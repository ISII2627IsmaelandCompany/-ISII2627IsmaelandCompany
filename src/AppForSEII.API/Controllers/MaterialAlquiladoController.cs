using AppForSEII.API.DTOs.MaterialAlquiladoDTOs; // DTOs del details de alquiler
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

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

        [HttpPost] // petición POST
        [Route("[action]")] // nombre del método en la URL
        [ProducesResponseType(typeof(MaterialAlquiladoDetailDTOs), (int)HttpStatusCode.Created)] // 201
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)] // 400
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)] // 409
        public async Task<ActionResult> CreateAlquiler(MaterialAlquiladoForCreateDTO alquilerForCreate)
        {
            // Flujo alternativo 3: debe haber al menos un material
            if (alquilerForCreate.Materiales.Count == 0)
                ModelState.AddModelError("Materiales", "Error, Debe seleccionar al menos un material");

            // No se puede repetir el mismo material
            if (alquilerForCreate.Materiales.GroupBy(m => m.MaterialId).Any(g => g.Count() > 1))
                ModelState.AddModelError("Materiales", "Error, No se puede seleccionar el mismo material dos veces");

            // Flujo alternativo 2: fecha anterior a la actual
            if (alquilerForCreate.FechaAlquiler.Date < DateTime.Today)
                ModelState.AddModelError("FechaAlquiler", "Error, La fecha del alquiler no puede ser anterior a la fecha actual");

            // Método de pago válido
            if (!Enum.IsDefined(alquilerForCreate.MetodoPago))
                ModelState.AddModelError("MetodoPago", "Error, El método de pago seleccionado no es válido");

            // Buscamos al usuario
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == alquilerForCreate.ClienteUserName);
            if (user == null)
                ModelState.AddModelError("ClienteUserName", "Error, El usuario no está registrado");

            // Flujo alternativo 5: datos obligatorios del formulario (vienen en el DTO, no del perfil)
            if (string.IsNullOrWhiteSpace(alquilerForCreate.DNI))
                ModelState.AddModelError("DNI", "Error, El DNI es obligatorio");
            if (string.IsNullOrWhiteSpace(alquilerForCreate.Nombre))
                ModelState.AddModelError("Nombre", "Error, El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(alquilerForCreate.Apellidos))
                ModelState.AddModelError("Apellidos", "Error, Los apellidos son obligatorios");
            if (string.IsNullOrWhiteSpace(alquilerForCreate.Telefono))
                ModelState.AddModelError("Telefono", "Error, El teléfono de contacto es obligatorio");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Cargamos los materiales con su tipo de material y tipo de deporte
            var materialesIds = alquilerForCreate.Materiales.Select(m => m.MaterialId).ToList();
            var materiales = await _context.Materials
                .Include(m => m.TipoMaterial)
                .Include(m => m.TipoDeporte)
                .Where(m => materialesIds.Contains(m.IdMaterial))
                .ToListAsync();

            // Validamos existencia, cantidad y stock de cada material
            foreach (var item in alquilerForCreate.Materiales)
            {
                var material = materiales.FirstOrDefault(m => m.IdMaterial == item.MaterialId);
                if (material == null)
                    ModelState.AddModelError("Materiales", $"Error, No existe el material con id {item.MaterialId}");
                else if (item.Cantidad < 1)
                    ModelState.AddModelError("Materiales", $"Error, La cantidad de {material.Nombre} debe ser al menos 1");
                else if (item.Cantidad > material.Cantidad)
                    ModelState.AddModelError("Materiales", $"Error, No hay suficientes unidades de {material.Nombre}");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Creamos el alquiler; el precio se calcula en el servidor
            var alquiler = new Alquiler(
                user!,
                alquilerForCreate.FechaAlquiler.Date,
                0,                              // idAlquiler: lo genera la BBDD
                new List<MaterialAlquilado>(),  // se rellena en el foreach
                alquilerForCreate.MetodoPago,
                0                               // precioTotal inicial, se recalcula después
            );

            foreach (var item in alquilerForCreate.Materiales)
            {
                var material = materiales.First(m => m.IdMaterial == item.MaterialId);

                var materialAlquilado = new MaterialAlquilado(
                    item.Cantidad,                                  // cantidad
                    alquilerForCreate.Descripcion ?? string.Empty,  // descripción opcional del cliente
                    material.IdMaterial,                            // idMaterial
                    0,                                              // idAlquiler: EF lo rellena por la relación
                    material.Precio * item.Cantidad                 // precio de la cantidad pedida
                )
                {
                    Material = material,
                    Alquiler = alquiler
                };

                alquiler.MaterialesAlquilados.Add(materialAlquilado);
                material.Cantidad -= item.Cantidad; // descontamos stock
            }

            // PrecioTotal es int en tu entidad y Precio es decimal, por eso el cast
            alquiler.PrecioTotal = (int)alquiler.MaterialesAlquilados.Sum(ma => ma.Precio);
            _context.Add(alquiler);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Conflict("Error, Ha ocurrido un error al guardar el alquiler: " + ex.Message);
            }

            // DTO de respuesta: datos del formulario + email del usuario
            var detail = new MaterialAlquiladoDetailDTOs(
                alquiler.IdAlquiler,
                alquilerForCreate.Nombre,
                alquilerForCreate.Apellidos,
                alquilerForCreate.DNI,
                user!.Email,
                alquilerForCreate.Telefono,
                alquiler.FechaAlquiler,
                alquiler.PrecioTotal,
                alquiler.MetodoPago,
                alquiler.MaterialesAlquilados
                    .Select(ma => new MaterialAlquiladoItemDTOs(
                        ma.Material.Nombre,
                        ma.Material.TipoMaterial.NombreTipoMaterial,
                        ma.Material.TipoDeporte.Nombre,
                        ma.Cantidad,
                        ma.Precio))
                    .ToList<MaterialAlquiladoItemDTOs>());

            return CreatedAtAction("GetAlquiler", new { id = alquiler.IdAlquiler }, detail);
        }
    }
}