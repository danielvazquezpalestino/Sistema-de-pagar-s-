using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApiDemo.DTOs.Auth;
using WebApiDemo.Models;
using WebApiDemo.Repositories;
using WebApiDemo.Patterns;

namespace WebApiDemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> LoginAsync(LoginRequest request, HttpContext httpContext)
        {
            Console.WriteLine($"[LOGIN] Intento de login para: {request.Correo}");
            Console.WriteLine($"[LOGIN] Nombre proporcionado: {request.Nombre ?? "null"}");
            Console.WriteLine($"[LOGIN] Rol proporcionado: {request.Rol ?? "null"}");

            // Mostrar todos los usuarios en la BD para depuración
            var todosUsuarios = await _usuarioRepository.GetAllAsync();
            Console.WriteLine($"[LOGIN] Total usuarios en BD: {todosUsuarios.Count()}");
            foreach (var u in todosUsuarios)
            {
                Console.WriteLine($"[LOGIN] - ID: {u.IdUsuario}, Correo: {u.Correo}, Nombre: {u.Nombre}, Rol: {u.Rol}");
            }

            var usuario = await _usuarioRepository.GetByCorreoAsync(request.Correo);
            Console.WriteLine($"[LOGIN] Usuario encontrado: {usuario != null}");

            // Si el usuario no existe, crear cuenta automáticamente
            if (usuario == null)
            {
                Console.WriteLine($"[LOGIN] Usuario no existe, intentando auto-registro");
                if (string.IsNullOrWhiteSpace(request.Nombre))
                {
                    Console.WriteLine($"[LOGIN] Auto-registro fallido: no se proporcionó nombre");
                    SingletonLogger.Instance.Log($"Intento de auto-registro sin nombre: {request.Correo}");
                    return false;
                }

                // Validar que el rol sea válido
                var rolValido = string.IsNullOrWhiteSpace(request.Rol) ? "abogado" : request.Rol;
                if (rolValido != "abogado" && rolValido != "administrador")
                {
                    Console.WriteLine($"[LOGIN] Auto-registro fallido: rol inválido: {request.Rol}");
                    SingletonLogger.Instance.Log($"Intento de auto-registro con rol inválido: {request.Correo} - Rol: {request.Rol}");
                    return false;
                }

                try
                {
                    // Crear nuevo usuario con rol especificado o "abogado" por defecto
                    usuario = new Usuario
                    {
                        Correo = request.Correo,
                        Nombre = request.Nombre,
                        Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
                        Rol = rolValido,
                        FechaRegistro = DateTime.UtcNow
                    };

                    usuario = await _usuarioRepository.CreateAsync(usuario);
                    Console.WriteLine($"[LOGIN] Usuario creado exitosamente: {usuario.IdUsuario}");
                    SingletonLogger.Instance.Log($"Usuario creado automáticamente: {request.Correo} ({request.Nombre}) - Rol: {rolValido}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LOGIN] Error al crear usuario: {ex.Message}");
                    SingletonLogger.Instance.Log($"Error al crear usuario automáticamente: {ex.Message}");
                    return false;
                }
            }
            else
            {
                // Usuario existe, validar contraseña
                Console.WriteLine($"[LOGIN] Usuario existe, validando contraseña");
                Console.WriteLine($"[LOGIN] Hash almacenado: {usuario.Contrasena}");
                bool isValid = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);
                Console.WriteLine($"[LOGIN] Contraseña válida: {isValid}");
                if (!isValid)
                {
                    SingletonLogger.Instance.Log($"Intento de login fallido (contraseña incorrecta): {request.Correo}");
                    return false;
                }
            }

            // Crear claims y login
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            Console.WriteLine($"[LOGIN] Ejutando SignInAsync...");
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            Console.WriteLine($"[LOGIN] SignInAsync ejecutado exitosamente");
            Console.WriteLine($"[LOGIN] Cookies después de SignIn: {httpContext.Response.Headers.SetCookie}");
            SingletonLogger.Instance.Log($"Login exitoso: {request.Correo}");
            return true;
        }

        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public MeResponse? GetCurrentUser(ClaimsPrincipal user)
        {
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated) return null;

            return new MeResponse
            {
                IdUsuario = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!),
                Nombre = user.FindFirstValue(ClaimTypes.Name)!,
                Correo = user.FindFirstValue(ClaimTypes.Email)!,
                Rol = user.FindFirstValue(ClaimTypes.Role)!
            };
        }
    }
}
