using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Models;
using WebApiDemo.Patterns;
using WebApiDemo.Repositories;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "administrador")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaRepository _auditoriaRepository;

        public AuditoriaController(IAuditoriaRepository auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
        }

        /// <summary>
        /// Get all audit records with optional filters - admin only
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Auditoria>>> GetAll(
            [FromQuery] int? idPagare = null,
            [FromQuery] int? idUsuario = null,
            [FromQuery] string? accion = null)
        {
            IEnumerable<Auditoria> auditorias;

            if (idPagare.HasValue)
            {
                auditorias = await _auditoriaRepository.GetByIdPagareAsync(idPagare.Value);
            }
            else if (idUsuario.HasValue)
            {
                auditorias = await _auditoriaRepository.GetByIdUsuarioAsync(idUsuario.Value);
            }
            else if (!string.IsNullOrEmpty(accion))
            {
                auditorias = await _auditoriaRepository.GetByAccionAsync(accion);
            }
            else
            {
                auditorias = await _auditoriaRepository.GetAllAsync();
            }

            SingletonLogger.Instance.Log($"GET /api/auditoria - Consultó auditoría con filtros: idPagare={idPagare}, idUsuario={idUsuario}, accion={accion}");
            
            return Ok(auditorias);
        }

        /// <summary>
        /// Get audit trail for a specific pagare - admin only
        /// </summary>
        [HttpGet("{idPagare}")]
        public async Task<ActionResult<IEnumerable<Auditoria>>> GetByIdPagare(int idPagare)
        {
            var auditorias = await _auditoriaRepository.GetByIdPagareAsync(idPagare);
            
            SingletonLogger.Instance.Log($"GET /api/auditoria/{idPagare} - Consultó auditoría del pagaré {idPagare}");
            
            return Ok(auditorias);
        }
    }
}
