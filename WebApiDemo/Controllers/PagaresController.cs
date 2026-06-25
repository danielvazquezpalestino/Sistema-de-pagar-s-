using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiDemo.DTOs.Pagares;
using WebApiDemo.Patterns;
using WebApiDemo.Services;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PagaresController : ControllerBase
    {
        private readonly IPagareService _pagareService;
        private readonly AppConfigurationService _appConfig;

        public PagaresController(IPagareService pagareService, AppConfigurationService appConfig)
        {
            _pagareService = pagareService;
            _appConfig = appConfig;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role)!;
        }

        /// <summary>
        /// Get all pagares - abogado sees only theirs, admin sees all
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagareResponseDto>>> GetAll()
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var pagares = await _pagareService.GetAllAsync(userId, userRole);
            
            SingletonLogger.Instance.Log($"GET /api/pagares - Usuario {userId} ({userRole}) listó {pagares.Count()} pagarés");
            
            return Ok(pagares);
        }

        /// <summary>
        /// Get a specific pagare by ID - registers CONSULTA in audit log
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PagareResponseDto>> GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                var pagare = await _pagareService.GetByIdAsync(id, userId, userRole);
                
                if (pagare == null)
                    return NotFound(new { error = "Pagaré no encontrado" });

                SingletonLogger.Instance.Log($"GET /api/pagares/{id} - Usuario {userId} consultó pagaré");
                
                return Ok(pagare);
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                SingletonLogger.Instance.Log($"DbUpdateException en GET /api/pagares/{id}: {ex.Message} | Inner: {inner}");
                return StatusCode(500, new { error = ex.Message, detalle = inner });
            }
            catch (Exception ex)
            {
                SingletonLogger.Instance.Log($"Error en GET /api/pagares/{id}: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new pagare - registers CREACION in audit log
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PagareResponseDto>> Create([FromBody] PagareCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                
                var pagare = await _pagareService.CreateAsync(dto, userId);
                
                SingletonLogger.Instance.Log($"POST /api/pagares - Usuario {userId} creó pagaré {pagare.IdPagare}");
                
                return CreatedAtAction(nameof(GetById), new { id = pagare.IdPagare }, pagare);
            }
            catch (Exception ex)
            {
                SingletonLogger.Instance.Log($"Error al crear pagaré: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Update an existing pagare - registers MODIFICACION in audit log
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PagareResponseDto>> Update(int id, [FromBody] PagareUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var pagare = await _pagareService.UpdateAsync(id, dto);
                
                if (pagare == null)
                    return NotFound(new { error = "Pagaré no encontrado" });

                var userId = GetCurrentUserId();
                SingletonLogger.Instance.Log($"PUT /api/pagares/{id} - Usuario {userId} modificó pagaré");
                
                return Ok(pagare);
            }
            catch (Exception ex)
            {
                SingletonLogger.Instance.Log($"Error al actualizar pagaré: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Delete a pagare - admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var success = await _pagareService.DeleteAsync(id);
                
                if (!success)
                    return NotFound(new { error = "Pagaré no encontrado" });

                var userId = GetCurrentUserId();
                SingletonLogger.Instance.Log($"DELETE /api/pagares/{id} - Usuario {userId} eliminó pagaré");
                
                return Ok(new { message = "Pagaré eliminado" });
            }
            catch (Exception ex)
            {
                SingletonLogger.Instance.Log($"Error al eliminar pagaré: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Print a pagare - registers IMPRESION in audit log
        /// </summary>
        [HttpPost("{id}/imprimir")]
        public async Task<ActionResult<PagareResponseDto>> Imprimir(int id)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var pagare = await _pagareService.GetByIdAsync(id, userId, userRole);
            
            if (pagare == null)
                return NotFound(new { error = "Pagaré no encontrado" });

            // Register IMPRESION action
            var auditoriaService = HttpContext.RequestServices.GetRequiredService<Services.IAuditoriaService>();
            await auditoriaService.RegistrarAsync(id, userId, "IMPRESION");
            
            SingletonLogger.Instance.Log($"POST /api/pagares/{id}/imprimir - Usuario {userId} imprimió pagaré");
            
            return Ok(pagare);
        }
    }
}
