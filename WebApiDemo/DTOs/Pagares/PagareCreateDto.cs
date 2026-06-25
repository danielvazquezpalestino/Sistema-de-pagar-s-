using System.ComponentModel.DataAnnotations;

namespace WebApiDemo.DTOs.Pagares
{
    public class PagareCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string NumeroExpediente { get; set; } = string.Empty;

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public string PromesaPago { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Beneficiario { get; set; } = string.Empty;

        [Required]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        [MaxLength(150)]
        public string LugarPago { get; set; } = string.Empty;

        [Required]
        public DateTime FechaElaboracion { get; set; }

        [Required]
        [MaxLength(150)]
        public string LugarSuscripcion { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Firma { get; set; } = string.Empty;
    }
}
