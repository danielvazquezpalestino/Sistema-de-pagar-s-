using System.ComponentModel.DataAnnotations;

namespace WebApiDemo.DTOs.Auth
{
    public class LoginRequest
    {
        [Required]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasena { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del usuario - requerido si es auto-registro
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Rol del usuario - requerido si es auto-registro. Valores: "abogado" o "administrador"
        /// </summary>
        public string? Rol { get; set; }
    }
}
