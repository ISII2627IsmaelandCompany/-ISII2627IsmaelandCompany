using AppForSEII.API.Data;
using AppForSEII.API.DTOs.InscripcionDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.EntityFrameworkCore;

using AppForSEII.API.Models;
using System.Linq.Expressions;
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
            _logger.LogInformation("InscripcionesController initialized");
            }
        [HttpGet]//acción para obtener las competiciones disponibles para la inscripción
        [Route("[action]")]//ruta de la acción
        [ProducesResponseType(typeof(IList<CompeticionForInscripcionDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]//tipos de respuesta que puede devolver la acción
        public async Task<ActionResult> GetCompeticionesForInscripcion(string? nombre, string? tipoDeporte, DateTime? fromDate, DateTime? toDate)//acción para obtener las competiciones disponibles para la inscripción, con filtros opcionales por nombre, tipo de deporte y rango de fechas
        {
            try{
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
            _logger.LogInformation(                $"{DateTime.Now} Info: Se han obtenido {competiciones.Count} competiciones disponibles para la inscripción");
            return Ok(competiciones);//devuelve un código 200 con la lista de competiciones disponibles para la inscripción
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las competiciones para la inscripción");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error al obtener las competiciones para la inscripción");
            }
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

[HttpPost]
[Route("[action]")]
[ProducesResponseType(typeof(InscripcionDetailDTO), (int)HttpStatusCode.Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType((int)HttpStatusCode.Conflict)]
public async Task<ActionResult> CrearInscripcion(CrearInscripcionDTO crearInscripcionDTO)//post de crear inscripcion
{
    //VALIDAMOS LOS DATOS RECIBIDOS (FLUJOS ALTERNATIVOS)
    // Comprobar que se ha seleccionado al menos una competición
    if (crearInscripcionDTO.CompeticionesSeleccionadas == null ||
        crearInscripcionDTO.CompeticionesSeleccionadas.Count == 0)
    {
        _logger.LogError(
            "Error: No se ha seleccionado ninguna competición para la inscripción");
        ModelState.AddModelError(
            "CompeticionesSeleccionadas",
            "Debe seleccionar al menos una competición.");

        return BadRequest(new ValidationProblemDetails(ModelState));
    }

    // Comprobar que no se ha seleccionado la misma competición varias veces
    var idsCompeticiones = crearInscripcionDTO.CompeticionesSeleccionadas
        .Select(c => c.IdCompeticion)
        .ToList();

    if (idsCompeticiones.Distinct().Count() != idsCompeticiones.Count)
    {
        _logger.LogError(
            "Error: Se ha seleccionado la misma competición varias veces para la inscripción");
        ModelState.AddModelError(
            "CompeticionesSeleccionadas",
            "No se puede seleccionar una misma competición varias veces.");

        return BadRequest(new ValidationProblemDetails(ModelState));
    }

    // Buscar al cliente por su nombre de usuario
    var cliente = await _context.Users
        .FirstOrDefaultAsync(u => u.UserName == crearInscripcionDTO.NombreUsuarioCliente);

    if (cliente == null)
    {
        _logger.LogError(
            $"No existe el cliente {crearInscripcionDTO.NombreUsuarioCliente}");

        return BadRequest();
    }

    // Obtener las competiciones seleccionadas desde la base de datos
    var competiciones = await _context.Competiciones
        .Where(c => idsCompeticiones.Contains(c.Id))
        .ToListAsync();

    // Comprobar que existen todas las competiciones solicitadas
    if (competiciones.Count != idsCompeticiones.Count)
    {
        ModelState.AddModelError(
            "CompeticionesSeleccionadas",
            "Una o varias competiciones no existen.");

        return BadRequest(new ValidationProblemDetails(ModelState));
    }
    //COMPROBACIONES
    // Comprobar que todas las competiciones tienen plazas disponibles
    foreach (var competicion in competiciones)
    {
        var plazasOcupadas = await _context.Set<CompeticionInscripcion>()
            .CountAsync(ci => ci.CompeticionId == competicion.Id);

        if (plazasOcupadas >= competicion.Plazas)
        {
            _logger.LogError(
                $"No quedan plazas disponibles para la competición {competicion.Nombre}");
            ModelState.AddModelError(
                "CompeticionesSeleccionadas",
                $"No quedan plazas disponibles para la competición {competicion.Nombre}.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }
    }

    // Calcular el precio total usando los precios almacenados en la base de datos
    decimal precioTotal = competiciones.Sum(c => c.Precio);

    // Crear la inscripción
    var inscripcion = new Inscripcion(
        cliente,
        string.Empty,
        DateTime.Now,
        crearInscripcionDTO.MetodoPago,
        precioTotal
    );

    // Asociar cada competición a la inscripción
    foreach (var competicion in competiciones)
    {
        var competicionSeleccionada = crearInscripcionDTO.CompeticionesSeleccionadas
            .First(c => c.IdCompeticion == competicion.Id);

        var relacion = new CompeticionInscripcion
        {
            CompeticionId = competicion.Id,
            Competicion = competicion,
            Inscripcion = inscripcion,
            ProblemasFisicos = competicionSeleccionada.ProblemasFisicos
        };

        inscripcion.CompeticionInscripciones.Add(relacion);
    }

    // Validar el estado del modelo antes de guardar
    if (!ModelState.IsValid)
    {
        _logger.LogError(
            "Error: El estado del modelo no es válido al crear la inscripción");
        return BadRequest(new ValidationProblemDetails(ModelState));
    }

    try
    {
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError(ex, "Error al guardar la inscripción.");

        return Conflict();
    }
    //INSERTAMOS LOS DATOS EN LA BDA
    // Preparar los datos de la inscripción creada
    var inscripcionDetail = new InscripcionDetailDTO(
        inscripcion.Id,
        cliente.Name,
        cliente.Surname,
        cliente.DNI,
        cliente.Email,
        cliente.PhoneNumber,
        inscripcion.FechaInscripcion,
        inscripcion.PrecioTotal,
        inscripcion.MetodoPago,
        inscripcion.CompeticionInscripciones
            .Select(ci => new InscripcionItemDTO(
                ci.Competicion.Nombre,
                ci.Competicion.Lugar,
                ci.Competicion.Fecha,
                ci.Competicion.Precio,
                ci.ProblemasFisicos
            ))
            .ToList()
    );
    //DEVOLVEMOS EL OK Y EL DETAIL
    _logger.LogInformation(
        $"Inscripción creada con éxito para el cliente {cliente.UserName} con ID {inscripcion.Id}");
    return CreatedAtAction(
        nameof(GetInscripcion),
        new { id = inscripcion.Id },
        inscripcionDetail
    );
}

    }
}

