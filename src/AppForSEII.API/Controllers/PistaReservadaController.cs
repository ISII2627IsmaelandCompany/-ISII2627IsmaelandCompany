using AppForSEII.API.DTOs.PistaReservadaDTOs;//Importar dto del details de  pista reservada
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]//Ruta del controlador
    [ApiController]//controlador de la api

    public class PistaReservadaController : ControllerBase
    {

        private readonly ApplicationDbContext _context;//para acceder a las tablas de la bbdd
        private readonly ILogger<PistaReservadaController> _logger;//registrar informacion o errores del controller

        public PistaReservadaController(ApplicationDbContext context,ILogger<PistaReservadaController> logger)
        {//constructor controller para proporcionar contexto e informacion
            _context = context;
            _logger = logger;
        }

        [HttpGet]//peticion HTTP GET
        [Route("[action]")]//nombre del metodo como parte de la  URL
        [ProducesResponseType(typeof(PistaReservadaDetailDTO), (int)HttpStatusCode.OK)]//todo correcto devuelve 200 OK e informacion
        [ProducesResponseType((int)HttpStatusCode.NotFound)]//va mal = 404 not found

        public async Task<ActionResult> GetPistaReservada(int idPista, int idReserva)//id de la reserva a consultar, la recibe por parametro
        {
            
            var Pistareservada=await _context.PistaReservada
            .Where(pr => pr.IdPista == idPista && pr.IdReserva == idReserva)//filtra hasta encontrar esa id
        .Include (pr => pr.Pista)// Cargamos el cliente de esta reserva
        .ThenInclude(p => p.TipoDeporte)// Para cada pista cargamos su tipo de deporte
            .Include(pr => pr.Reserva)//para cada pista reservada cargamos la reserva
            .ThenInclude(r => r.Cliente)//para cada reserva cargamos el cliente
        .Include(pr => pr.Reserva)
        .ThenInclude(r => r.PistasReservadas)//para cada reserva cargamos las pistas reservadas
        .ThenInclude(pRes => pRes.Pista)//para cada pista reservada cargamos la pista
        .ThenInclude(p => p.TipoDeporte)//para cada pista cargamos el tipo de deporte
            //dto que queremos devolver al cliente
    .Select(pr => new PistaReservadaDetailDTO(pr.Reserva.Id,pr.Reserva.Cliente.Name,pr.Reserva.Cliente.Surname,pr.Reserva.Cliente.DNI, pr.Reserva.Cliente.Email,pr.Reserva.Cliente.PhoneNumber,pr.Reserva.FechaReserva,pr.Reserva.PrecioTotal,pr.Reserva.MetodoPago,
    pr.Reserva.PistasReservadas.Select(pRes => new PistaReservadaItemDTO(pRes.IdPista,pRes.Pista.NombrePista,pRes.Pista.TipoDeporte.Nombre,pRes.Observaciones,pRes.Precio,pRes.Cantidad)).ToList())).FirstOrDefaultAsync();
          
    if (Pistareservada == null)            
    {
        _logger.LogError($"No se ha encontrado la pista reservada con  con id IdPista {idPista} e IdReserva {idReserva}");
        return NotFound();
    }

    return Ok(Pistareservada);//sino 200 e informacion reserva
        }
    
            [HttpPost]//peticion post
        [Route("[action]")]//nombre del metodo en la ruta
        [ProducesResponseType(typeof(PistaReservadaDetailDTO), (int)HttpStatusCode.Created)]//si va todo bien se crea 201
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]//usuario inexistente, pista inexistente, aforo superado...(errores)
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]//error de conflicto si a la hora de guardar en la base  de datos da problemas
        public async Task<ActionResult> CreatePistaReservada(PistaReservadaForCreateDTO reservaForCreate)//metodo que devolvera de forma asincrona 201 OK 400 badrequest o 409 conflicto y el parametro es el json  que manda el cliente para hacer la reserva
        {
        // comprobamos que se selecciona al menos una pista
        if (reservaForCreate.PistasReservadas.Count == 0)
        {//si no, error
         ModelState.AddModelError("PistasReservadas", "Error, Debe incluir al menos una pista");
        }    
        // comprobamos que no se haya seleccionado dos veces la misma pista
        if (reservaForCreate.PistasReservadas
        .GroupBy(pr => pr.IdPista)//agrupa los elemetos que tienen el mismo id de pista
        .Any(grupo => grupo.Count() > 1))//comprueba si existe algun grupo repetido, si es asi devuelve true y entra en el if
        {
        // si hay una pista repetida, añadimos un error
        ModelState.AddModelError("PistasReservadas","Error, No se puede seleccionar la misma pista dos veces");
        }
    
        // buscamos al usuario por su nombre de usuario
        var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == reservaForCreate.ClienteUserName); //devuelve o null o el cliente

        // si el usuario no existe error
        if (user == null)
        {
        ModelState.AddModelError("ReservaApplicationUser","Error, El usuario no está registrado");
        }

        // comprobamos que el metodo de pago exista en nuestro enum
        if (!Enum.IsDefined(reservaForCreate.MetodoPago))
        {
        // si no es valido añadimos un error
        ModelState.AddModelError("MetodoPago","Error, El metodo de pago seleccionado no es valido");  
        }
    
        // si hay algún error , tipo  no ha seleccionado pista o el usuario no existe entra en el if
        if (ModelState.ErrorCount > 0)
        {
        return BadRequest(new ValidationProblemDetails(ModelState));//se sale del metodo y devolvemos 400 Bad request
        }
    
        // obtenemos los id de las pistas que ha seleccionado el cliente
        var pistasIds = reservaForCreate.PistasReservadas.Select(pr => pr.IdPista).ToList();//filtramos solo por los ids y transformamos a lista

    
    
        // buscamos las pistas en la base de datos y cargamos también su tipo de deporte
        var pistas = await _context.Pistas
        .Include(p => p.TipoDeporte)
        .Where(p => pistasIds.Contains(p.IdPista))
        .ToListAsync();    
        
        // comprobamos que cada pista existe y tiene suficientes plazas (validar)
        foreach (var item in reservaForCreate.PistasReservadas)//por cada pista  que el cliente quiere reservar
    {   //item es cada una de  esas pistas

        var pista = pistas.FirstOrDefault(p => p.IdPista == item.IdPista);//buscamos la pista en la base de datos
        if(item.Cantidad<1)
        {
        ModelState.AddModelError("PistasReservadas", "Error, Debe reservar para al menos 1 persona");
        }
        else if (pista == null)//si no existe
        {
            ModelState.AddModelError("PistasReservadas",$"Error,no existe la pista con id {item.IdPista} ");            
        }
        else if (_context.PistaReservada.Any(pr => pr.IdPista == item.IdPista && pr.Reserva.FechaReserva.Date == DateTime.Today))
        {
        ModelState.AddModelError("PistasReservadas",
        $"Error, la pista {pista.NombrePista} ya está reservada para hoy");
        }
        else if(item.Cantidad > pista.NPersonas)//si la cantidad que quiere reservar es mayor que el aforo de la pista
        {
            ModelState.AddModelError("PistasReservadas",$"Error, no hay suficientes plazas en la pista {pista.NombrePista} ");       

        } 
    
    }
    
    if (ModelState.ErrorCount > 0)//si aparecio algun error  por pista no existente o por plazas
    {
    return BadRequest(new ValidationProblemDetails(ModelState));//mostramos bad request y  paramos
    }

    // creamos la reserva con los datos recibidos
    var reserva = new Reserva(user!,//la exclamacion es para decir que no sera nullo el usuario ya esta comprobado en el if de la linea 67
    reservaForCreate.MetodoPago,//metodo de pago elegido por el cliente
    DateTime.Now,//fecha y hora en la que se realiza la reserva  (por  el sistema se realiza)   
    0m//precio total inicial m=decimal, el precio se calcula mas adelante
    );

// volvemos a recorremos las pistas seleccionadas para crear las pistas reservadas y calcular el precio total
foreach (var item in reservaForCreate.PistasReservadas)
{//usamos first porque si ha  llegado hasta aqui  no  habra  ningun null o dato  no encontrado
    var pista = pistas.First(p => p.IdPista == item.IdPista);

    // calculamos el precio real usando el precio de la base de datos
    item.Precio = pista.Precio;

    // Crear pista reservada
    var pistaReservada = new PistaReservada(pista, reserva, item.Cantidad, item.Precio);
        pistaReservada.Observaciones = item.Observaciones;
    // añadimos la pista reservada a la reserva
    reserva.PistasReservadas.Add(pistaReservada);

}//fin for each

// calculamos el precio total de la reserva  de todas las pistas reservadas
reserva.PrecioTotal = (decimal)reserva.PistasReservadas.Sum(pr => pr.Precio);
    // añadimos la reserva al contexto
_context.Add(reserva);

try
{
    // guardamos la reserva y sus pistas reservadas en la base de datos
    await _context.SaveChangesAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex.Message);//guardamos mensaje log

    ModelState.AddModelError("Reserva","Error, Ha ocurrido un error al guardar la reserva");

    return Conflict("Error" + ex.Message);//devolvemos 409  conflict 
}

// Creo el DTO que vamos a devolver con la reserva creada
            var reservaDetail = new PistaReservadaDetailDTO(
                reserva.Id,
                user!.Name,
                user.Surname,
                user.DNI,
                user.Email,
                user.PhoneNumber,
                reserva.FechaReserva,
                reserva.PrecioTotal,
                reserva.MetodoPago,
            // creamos la lista del resumen con los datos reales de las  pistas reservadas que se han guardado en la base de datos
                reserva.PistasReservadas
             .Select(pr => new PistaReservadaItemDTO(
                        pr.Pista.IdPista,       //id
                        pr.Pista.NombrePista,//nombre
                        pr.Pista.TipoDeporte.Nombre,    //tipo de deporte
                        pr.Observaciones, //observaciones
                        pr.Precio,//precio
                        pr.Cantidad//cantidad
                    ))
                    .ToList());

        // devolvemos 201 Created y el detalle de la reserva
        return CreatedAtAction(
        "GetPistaReservada",//nombre del metodo que devuelve la reserva creada
        new { idPista = reserva.PistasReservadas.First().IdPista, idReserva = reserva.Id },//parametros que necesita el metodo Get para devolver la reserva creada
        reservaDetail
        );}
    }   
}
