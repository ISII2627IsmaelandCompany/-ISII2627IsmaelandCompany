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
[HttpPost]//peticion post 
[Route("[action]")]//nombre  del metodo en la ruta
[ProducesResponseType(typeof(InscripcionClaseDeportivaDetailDTO), (int)HttpStatusCode.Created)]//si va todo bien se crea  201
[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]//sino se ha podido crear inscripcion error por usuario inexsitente, clase inexistente, no hay plazas
[ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]//error de conflicto si a la hora de guardar en la base  de datos da problemas
public async Task<ActionResult> CreateInscripcion(InscripcionClaseDeportivaForCreateDTO inscripcionForCreate)//metodo que devolvera de forma asincrona 201 OK 400 badrequest o 409 conflicto y el parametro es el json  que manda el cliente para inscribirse
{
// seleccionar al menos una clase deportiva  sino error
if (inscripcionForCreate.ClasesInscritas.Count == 0)
{
    ModelState.AddModelError("ClasesInscritas", "Error, Debe incluir al menos una clase deportiva");
}

// buscamos al usuario por su nombre de usuario
var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == inscripcionForCreate.ClienteUserName); //devuelve o null o el cliente

// si el usuario no existe error
if (user == null)
{
    ModelState.AddModelError("InscripcionApplicationUser","Error, El usuario no está registrado");
}

// si hay algún error , tipo  no ha seleccionado clase deportiva o el usuario no existe entra en el if
if (ModelState.ErrorCount > 0)
{
    return BadRequest(new ValidationProblemDetails(ModelState));//se sale del metodo y devolvemos 400 Bad request
}
   // obtenemos los id de las clases que ha seleccionado el cliente
var clasesIds = inscripcionForCreate.ClasesInscritas.Select(ci => ci.ClaseDeportivaId).ToList();//filtramos solo por los ids y transformamos a lista

// buscamos esas clases en la base de datos
var clasesDeportivas = await _context.ClasesDeportivas.Where(cd => clasesIds.Contains(cd.Id)).ToListAsync();//buscamos en la base  de datos los ids  de  las clases deportivas  que ha generado la lista
    // comprobamos que cada clase existe y tiene suficientes plazas (validar)
foreach (var item in inscripcionForCreate.ClasesInscritas)//por cada clase  que el cliente quiere reservar
{//item es cada una de  esas clases
    var clase = clasesDeportivas.FirstOrDefault(cd => cd.Id == item.ClaseDeportivaId);//buscamos la clase correspondiente al item con su id

    // la clase no existe
    if (clase == null)
    {
        ModelState.AddModelError("ClasesInscritas",$"Error, No existe la clase deportiva con id {item.ClaseDeportivaId}");
    }
    // no hay suficientes plazas
    else if (item.PlazasReservadas > clase.PlazasDisponibles)
    {
        ModelState.AddModelError("ClasesInscritas",$"Error, No hay suficientes plazas disponibles para la clase {clase.Descripcion}");

    }
    
}
    if (ModelState.ErrorCount > 0)//si aparecio algun error tipo por clase no existente o por plazas
    {
    return BadRequest(new ValidationProblemDetails(ModelState));//mostramos bad request y  paramos
    }
    // creamos la inscripción con los datos recibidos
var inscripcion = new Inscripcion(user!,//la exclamacion es para decir que no sera nullo el usuario ya esta comprobado en if ln 60
inscripcionForCreate.DatosPago,
DateTime.Now//fecha y hora en la que se realiza inscripcion  (por  el sistema se realiza)
,inscripcionForCreate.MetodoPago//Tarjeta Bizum Efectivo
,0m//precio total inicial m=decimal, el precio se calcula mas adelante
);
// volvemos a recorremos las clases seleccionadas (crear)
foreach (var item in inscripcionForCreate.ClasesInscritas)
{//usamos first porque si ha  llegado hasta aqui  no  habra  ningun null o dato  no encontrado
    var clase = clasesDeportivas.First(cd => cd.Id == item.ClaseDeportivaId);

    // calculamos el precio real usando el precio de la base de datos
    item.Precio = clase.PrecioUnitario * item.PlazasReservadas;

    // Crear clase inscrita
    var claseInscrita = new ClaseInscrita(
        item.Observaciones,//opcional
        item.PlazasReservadas,//plazas
        item.Precio//precio calculado
    )
    {
        ClaseDeportiva = clase,//clase deportiva se  le asigna la que se inscribio el cliente
        Inscripcion = inscripcion,//clase inscrita  con la inscripcion  creada
        ClaseDeportivaId = clase.Id//relacionamos con la clase deportiva
    };

    // añadimos la clase inscrita a la inscripción
    inscripcion.ClasesInscritas.Add(claseInscrita);

    // descontamos las plazas reservadas
    clase.PlazasDisponibles = clase.PlazasDisponibles - item.PlazasReservadas;
}//fin for each
// calculamos el precio total de la inscripción de todas las clases inscritas
inscripcion.PrecioTotal = inscripcion.ClasesInscritas
    .Sum(ci => ci.Precio);
    // añadimos la inscripción al contexto
_context.Add(inscripcion);

try
{
    // guardamos la inscripción y sus clases inscritas en la base de datos
    await _context.SaveChangesAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex.Message);//guardamos mensaje log

    ModelState.AddModelError("Inscripcion","Error, Ha ocurrido un error al guardar la inscripción");

    return Conflict("Error" + ex.Message);//devolvemos 409  conflict 
}
// creamos el DTO que vamos a devolver con la inscripción creada
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

// devolvemos 201 Created y el detalle de la inscripción
return CreatedAtAction(
    "GetInscripcion",
    new { id = inscripcion.Id },
    inscripcionDetail
);}
    }
}