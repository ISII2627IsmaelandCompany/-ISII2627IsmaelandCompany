using AppForSEII.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoDeportesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TipoDeportesController> _logger;

        public TipoDeportesController(
            ApplicationDbContext context,
            ILogger<TipoDeportesController> logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}