using AppForSEII.API.DTOs.ClaseDeportivaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers//sitio  donde esta el controlador
{
    [Route("api/[controller]")]//ruta base  del controlador: api/Clases Deportivas
    [ApiController]//controlador de  una api
    public class ClasesDeportivasController:ControllerBase
    {
        private readonly ApplicationDbContext _context;//contex se usara para acceder a base de datos
        private readonly ILogger<ClasesDeportivasController> _logger;//registrar informacion o  errores

        public ClasesDeportivasController(ApplicationDbContext context, ILogger<ClasesDeportivasController> logger)
        {//constructor controlador
            _context = context;
            _logger = logger;
        }
        [HttpGet]//peticiones HTTP Get
        [Route("[action]")]//la api usa nombre metodo: api/ClasesDeportivas/GetClasesDeportivasForInscripcion
[ProducesResponseType(typeof(IList<ClaseDeportivaForInscripcionDTO>), (int)HttpStatusCode.OK)]//si todo va correcto devuelve lista de ClaseDeportivaForInscripcionDto
public async Task<ActionResult> GetClasesDeportivasForInscripcion(string? tipoDeporte,string? monitor,DateTime? fecha){ 
    IList<ClaseDeportivaForInscripcionDTO> clases = await _context.ClasesDeportivas//consultar  clase deportivas  de la base  de datos

        .Include(c => c.TipoDeporte)//carga informacion relaciona de tipodeporte para devolver su nombre

        .Where(c =>c.PlazasDisponibles > 0 && (tipoDeporte == null || c.TipoDeporte.Nombre.Contains(tipoDeporte))
            && (monitor == null || c.Monitor.Contains(monitor))
            && (fecha == null || c.FechaHora.Date == fecha.Value.Date)
        )//filtramos clases mostrar, solo las que tengan plazas disponibles, si tipodeporte null no filtramos
        //si tiene  valor comprobamos que el nombre del deporte lo contenga
        //al igual con  monitor y fecha que si es  null nada , solo si se  encuentra su contenido

        .OrderBy(c => c.FechaHora)//Ordenamos por fecha y hora

        .Select(c => new ClaseDeportivaForInscripcionDTO(c.Id,c.Descripcion,c.TipoDeporte.Nombre,c.FechaHora,c.Nivel,c.Monitor,c.Lugar,c.PlazasDisponibles,c.PrecioUnitario))
        //convertimos claseDeportiva de la base  de datos en  nuesto Dto, devolviendo datos  necesarios
        .ToListAsync();//convertimos resultado en  una lista

    return Ok(clases);}
    }

    
}