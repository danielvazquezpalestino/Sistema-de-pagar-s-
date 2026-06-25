using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiDemo.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("nombre")]
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Column("correo")]
        [Required, MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Column("contrasena")]
        [Required, MaxLength(255)]
        public string Contrasena { get; set; } = string.Empty; // BCrypt hash

        [Column("rol")]
        [Required]
        public string Rol { get; set; } = string.Empty; // "abogado" | "administrador"

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        public ICollection<Pagare> Pagares { get; set; } = new List<Pagare>();
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}
