using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs;//Importar dto del details de inscripcion clase deportiva
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
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
            .Select(ci => new InscripcionClaseDeportivaItemDTO(ci.ClaseDeportiva.Id,ci.ClaseDeportiva.Descripcion,ci.ClaseDeportiva.TipoDeporte.Nombre,ci.ClaseDeportiva.FechaHora,ci.PlazasReservadas,ci.Precio,ci.Observaciones)).ToList<InscripcionClaseDeportivaItemDTO>())).FirstOrDefaultAsync();
            //el id , datos del cliente , datos inscripcion , datos clase inscrita , datos clase deportiva. todo esto se devuelve en lista
            //entonces al final el firtordefault devuelve o null o toda la lista de inscripcion transformado a dto
             if (inscripcion == null)//si es null entra y mensaje de error mas el 404
    {
        _logger.LogError($"No se ha encontrado la inscripción con id {id}");
        return NotFound();
    }

    return Ok(inscripcion);//sino 200 e informacion inscripcion
        }
        [HttpPost]
[Route("[action]")]
[ProducesResponseType(typeof(InscripcionClaseDeportivaDetailDTO), (int)HttpStatusCode.Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
public async Task<ActionResult> CreateInscripcion(
    InscripcionClaseDeportivaForCreateDTO inscripcionForCreate)
{
// Comprobamos que el cliente haya seleccionado al menos una clase
if (inscripcionForCreate.ClasesInscritas.Count == 0)
{
    ModelState.AddModelError(
        "ClasesInscritas",
        "Error! Debe incluir al menos una clase deportiva");
}

// Buscamos al usuario por su nombre de usuario/email
var user = _context.ApplicationUsers
    .FirstOrDefault(au => au.UserName == inscripcionForCreate.ClienteUserName);

// Si el usuario no existe añadimos error al ModelState
if (user == null)
{
    ModelState.AddModelError(
        "InscripcionApplicationUser",
        "Error! El usuario no está registrado");
}

// Si hay algún error devolvemos 400 Bad Request
if (ModelState.ErrorCount > 0)
{
    return BadRequest(new ValidationProblemDetails(ModelState));
}
   // Obtenemos los ids de las clases que ha seleccionado el cliente
var clasesIds = inscripcionForCreate.ClasesInscritas
    .Select(ci => ci.ClaseDeportivaId)
    .ToList();

// Buscamos esas clases en la base de datos
var clasesDeportivas = await _context.ClasesDeportivas
    .Where(cd => clasesIds.Contains(cd.Id))
    .ToListAsync();
    // Comprobamos que cada clase existe y tiene suficientes plazas
foreach (var item in inscripcionForCreate.ClasesInscritas)
{
    var clase = clasesDeportivas
        .FirstOrDefault(cd => cd.Id == item.ClaseDeportivaId);

    // La clase no existe
    if (clase == null)
    {
        ModelState.AddModelError(
            "ClasesInscritas",
            $"Error! No existe la clase deportiva con id {item.ClaseDeportivaId}");
    }
    // No hay suficientes plazas
    else if (item.PlazasReservadas > clase.PlazasDisponibles)
    {
        ModelState.AddModelError(
            "ClasesInscritas",
            $"Error! No hay suficientes plazas disponibles para la clase {clase.Descripcion}");
    }
    
}
if (ModelState.ErrorCount > 0)
{
    return BadRequest(new ValidationProblemDetails(ModelState));
}
// Creamos la inscripción con los datos recibidos
var inscripcion = new Inscripcion(
    user!,
    inscripcionForCreate.DatosPago,
    DateTime.Now,
    inscripcionForCreate.MetodoPago,
    0m
);
// Recorremos las clases seleccionadas
foreach (var item in inscripcionForCreate.ClasesInscritas)
{
    var clase = clasesDeportivas
        .First(cd => cd.Id == item.ClaseDeportivaId);

    // Calculamos el precio real usando el precio de la base de datos
    item.Precio = clase.PrecioUnitario * item.PlazasReservadas;

    // Creamos la relación entre la inscripción y la clase deportiva
    var claseInscrita = new ClaseInscrita(
        item.Observaciones,
        item.PlazasReservadas,
        item.Precio
    )
    {
        ClaseDeportiva = clase,
        Inscripcion = inscripcion,
        ClaseDeportivaId = clase.Id
    };

    // Añadimos la clase inscrita a la inscripción
    inscripcion.ClasesInscritas.Add(claseInscrita);

    // Descontamos las plazas reservadas
    clase.PlazasDisponibles -= item.PlazasReservadas;
}
// Calculamos el precio total de la inscripción
inscripcion.PrecioTotal = inscripcion.ClasesInscritas
    .Sum(ci => ci.Precio);
    // Añadimos la inscripción al contexto
_context.Add(inscripcion);

try
{
    // Guardamos la inscripción y sus clases inscritas en la base de datos
    await _context.SaveChangesAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex.Message);

    ModelState.AddModelError(
        "Inscripcion",
        "Error! Ha ocurrido un error al guardar la inscripción");

    return Conflict("Error" + ex.Message);
}
// Creamos el DTO que vamos a devolver con la inscripción creada
var inscripcionDetail = new InscripcionClaseDeportivaDetailDTO(
    inscripcion.Id,
    user!.Name,
    user.Surname,
    user.DNI,
    user.Email,
    user.PhoneNumber,
    inscripcion.FechaInscripcion,
    inscripcion.PrecioTotal,
    inscripcion.MetodoPago,
    inscripcionForCreate.ClasesInscritas
);

// Devolvemos 201 Created y el detalle de la inscripción
return CreatedAtAction(
    "GetInscripcion",
    new { id = inscripcion.Id },
    inscripcionDetail
);}
    }
}