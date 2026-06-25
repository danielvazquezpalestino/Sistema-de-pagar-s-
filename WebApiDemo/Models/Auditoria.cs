using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiDemo.Models
{
    [Table("auditoria")]
    public class Auditoria
    {
        [Key]
        [Column("id_auditoria")]
        public int IdAuditoria { get; set; }

        [Column("id_pagare")]
        [Required]
        public int IdPagare { get; set; }

        [Column("id_usuario")]
        [Required]
        public int IdUsuario { get; set; }

        [Column("accion")]
        [Required]
        public string Accion { get; set; } = string.Empty; // "CREACION", "MODIFICACION", "CONSULTA", "IMPRESION"

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [ForeignKey("IdPagare")]
        public Pagare Pagare { get; set; } = null!;

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; } = null!;
    }
}
