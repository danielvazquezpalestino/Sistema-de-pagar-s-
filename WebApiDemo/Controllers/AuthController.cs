using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiDemo.DTOs.Auth;
using WebApiDemo.Services;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login endpoint - validates credentials and sets authentication cookie
        /// Si el usuario no existe, lo crea automáticamente (auto-registro)
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.LoginAsync(request, HttpContext);
            
            if (!success)
                return Unauthorized(new { error = "Credenciales inválidas. Si es auto-registro, proporcione los campos 'Nombre' y 'Rol'" });

            return Ok(new LoginResponse { Success = true, Message = "Login exitoso" });
        }

        /// <summary>
        /// Logout endpoint - clears authentication cookie
        /// </summary>
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _authService.LogoutAsync(HttpContext);
            return Ok(new { message = "Logout exitoso" });
        }

        /// <summary>
        /// Get current authenticated user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<MeResponse> Me()
        {
            var user = _authService.GetCurrentUser(User);
            if (user == null)
                return Unauthorized(new { error = "No autenticado" });

            return Ok(user);
        }
    }
}
