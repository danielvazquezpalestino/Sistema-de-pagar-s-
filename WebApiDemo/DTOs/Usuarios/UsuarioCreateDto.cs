using System.ComponentModel.DataAnnotations;

namespace WebApiDemo.DTOs.Usuarios
{
    public class UsuarioCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasena { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = string.Empty; // "abogado" | "administrador"
    }
}
