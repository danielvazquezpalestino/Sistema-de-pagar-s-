using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiDemo.Models
{
    [Table("pagares")]
    public class Pagare
    {
        [Key]
        [Column("id_pagare")]
        public int IdPagare { get; set; }

        [Column("numero_expediente")]
        [Required, MaxLength(50)]
        public string NumeroExpediente { get; set; } = string.Empty;

        [Column("monto")]
        [Required]
        public decimal Monto { get; set; }

        [Column("promesa_pago")]
        [Required]
        public string PromesaPago { get; set; } = string.Empty;

        [Column("beneficiario")]
        [Required, MaxLength(150)]
        public string Beneficiario { get; set; } = string.Empty;

        [Column("fecha_vencimiento")]
        [Required]
        public DateTime FechaVencimiento { get; set; }

        [Column("lugar_pago")]
        [Required, MaxLength(150)]
        public string LugarPago { get; set; } = string.Empty;

        [Column("fecha_elaboracion")]
        [Required]
        public DateTime FechaElaboracion { get; set; }

        [Column("lugar_suscripcion")]
        [Required, MaxLength(150)]
        public string LugarSuscripcion { get; set; } = string.Empty;

        [Column("firma")]
        [Required, MaxLength(255)]
        public string Firma { get; set; } = string.Empty;

        [Column("id_usuario")]
        [Required]
        public int IdUsuario { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; } = null!;

        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}
