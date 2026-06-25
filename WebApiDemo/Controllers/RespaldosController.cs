using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using WebApiDemo.Models;
using WebApiDemo.Patterns;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "administrador")]
    public class RespaldosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RespaldosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all backup records - admin only
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Respaldo>>> GetAll()
        {
            var respaldos = await _context.Respaldos.OrderByDescending(r => r.Fecha).ToListAsync();
            
            SingletonLogger.Instance.Log($"GET /api/respaldos - Listó {respaldos.Count()} registros de respaldo");
            
            return Ok(respaldos);
        }

        /// <summary>
        /// Create a backup record - admin only
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Respaldo>> Create([FromBody] RespaldoCreateDto dto)
        {
            var respaldo = new Respaldo
            {
                Fecha = DateTime.UtcNow,
                Descripcion = dto.Descripcion
            };

            _context.Respaldos.Add(respaldo);
            await _context.SaveChangesAsync();

            SingletonLogger.Instance.Log($"POST /api/respaldos - Creó registro de respaldo {respaldo.IdRespaldo}");
            
            return CreatedAtAction(nameof(GetAll), new { id = respaldo.IdRespaldo }, respaldo);
        }
    }

    public class RespaldoCreateDto
    {
        public string? Descripcion { get; set; }
    }
}
