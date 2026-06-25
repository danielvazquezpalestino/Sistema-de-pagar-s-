using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiDemo.DTOs.Usuarios;
using WebApiDemo.Models;
using WebApiDemo.Patterns;
using WebApiDemo.Repositories;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        /// <summary>
        /// Get all users - admin only
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetAll()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            
            var dtos = usuarios.Select(u => new UsuarioResponseDto
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Correo = u.Correo,
                Rol = u.Rol,
                FechaRegistro = u.FechaRegistro
            });

            SingletonLogger.Instance.Log($"GET /api/usuarios - Listó {dtos.Count()} usuarios");
            
            return Ok(dtos);
        }

        /// <summary>
        /// Get a specific user by ID - admin only
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetById(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            
            if (usuario == null)
                return NotFound(new { error = "Usuario no encontrado" });

            var dto = new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                FechaRegistro = usuario.FechaRegistro
            };

            SingletonLogger.Instance.Log($"GET /api/usuarios/{id} - Consultó usuario {id}");
            
            return Ok(dto);
        }

        /// <summary>
        /// Create a new user - admin only, password is hashed with BCrypt
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> Create([FromBody] UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existing = await _usuarioRepository.GetByCorreoAsync(dto.Correo);
            if (existing != null)
                return BadRequest(new { error = "El correo ya está registrado" });

            // Hash password with BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contrasena = hashedPassword,
                Rol = dto.Rol,
                FechaRegistro = DateTime.UtcNow
            };

            var created = await _usuarioRepository.CreateAsync(usuario);

            var responseDto = new UsuarioResponseDto
            {
                IdUsuario = created.IdUsuario,
                Nombre = created.Nombre,
                Correo = created.Correo,
                Rol = created.Rol,
                FechaRegistro = created.FechaRegistro
            };

            SingletonLogger.Instance.Log($"POST /api/usuarios - Creó usuario {created.IdUsuario}");
            
            return CreatedAtAction(nameof(GetById), new { id = created.IdUsuario }, responseDto);
        }

        /// <summary>
        /// Update a user - admin only, password is hashed if provided
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> Update(int id, [FromBody] UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { error = "Usuario no encontrado" });

            // Check if email is being changed and if it already exists
            if (usuario.Correo != dto.Correo)
            {
                var existing = await _usuarioRepository.GetByCorreoAsync(dto.Correo);
                if (existing != null)
                    return BadRequest(new { error = "El correo ya está registrado" });
            }

            usuario.Nombre = dto.Nombre;
            usuario.Correo = dto.Correo;
            usuario.Rol = dto.Rol;

            // Hash password if provided (non-empty)
            if (!string.IsNullOrEmpty(dto.Contrasena))
            {
                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            }

            var updated = await _usuarioRepository.UpdateAsync(usuario);
            if (updated == null)
                return NotFound(new { error = "Error al actualizar usuario" });

            var responseDto = new UsuarioResponseDto
            {
                IdUsuario = updated.IdUsuario,
                Nombre = updated.Nombre,
                Correo = updated.Correo,
                Rol = updated.Rol,
                FechaRegistro = updated.FechaRegistro
            };

            SingletonLogger.Instance.Log($"PUT /api/usuarios/{id} - Actualizó usuario {id}");
            
            return Ok(responseDto);
        }

        /// <summary>
        /// Delete a user - admin only
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _usuarioRepository.DeleteAsync(id);
            
            if (!success)
                return NotFound(new { error = "Usuario no encontrado" });

            SingletonLogger.Instance.Log($"DELETE /api/usuarios/{id} - Eliminó usuario {id}");
            
            return Ok(new { message = "Usuario eliminado" });
        }
    }
}
