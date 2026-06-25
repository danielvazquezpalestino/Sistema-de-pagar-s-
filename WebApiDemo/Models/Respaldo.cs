using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiDemo.Models
{
    [Table("respaldos")]
    public class Respaldo
    {
        [Key]
        [Column("id_respaldo")]
        public int IdRespaldo { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("descripcion")]
        [MaxLength(255)]
        public string? Descripcion { get; set; }
    }
}
