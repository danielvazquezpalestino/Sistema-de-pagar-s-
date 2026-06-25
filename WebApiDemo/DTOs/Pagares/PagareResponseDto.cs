namespace WebApiDemo.DTOs.Pagares
{
    public class PagareResponseDto
    {
        public int IdPagare { get; set; }
        public string NumeroExpediente { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string PromesaPago { get; set; } = string.Empty;
        public string Beneficiario { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public string LugarPago { get; set; } = string.Empty;
        public DateTime FechaElaboracion { get; set; }
        public string LugarSuscripcion { get; set; } = string.Empty;
        public string Firma { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
