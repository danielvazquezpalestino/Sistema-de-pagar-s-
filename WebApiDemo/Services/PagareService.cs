using WebApiDemo.DTOs.Pagares;
using WebApiDemo.Models;
using WebApiDemo.Repositories;

namespace WebApiDemo.Services
{
    public class PagareService : IPagareService
    {
        private readonly IPagareRepository _pagareRepository;
        private readonly IAuditoriaService _auditoriaService;

        public PagareService(IPagareRepository pagareRepository, IAuditoriaService auditoriaService)
        {
            _pagareRepository = pagareRepository;
            _auditoriaService = auditoriaService;
        }

        public async Task<PagareResponseDto?> GetByIdAsync(int id, int currentUserId, string currentUserRole)
        {
            var pagare = await _pagareRepository.GetByIdAsync(id);
            if (pagare == null) return null;

            // Role-based access: abogado can only see their own, admin can see all
            if (currentUserRole == "abogado" && pagare.IdUsuario != currentUserId)
                return null;

            await _auditoriaService.RegistrarAsync(id, currentUserId, "CONSULTA");

            return MapToDto(pagare);
        }

        public async Task<IEnumerable<PagareResponseDto>> GetAllAsync(int currentUserId, string currentUserRole)
        {
            IEnumerable<Pagare> pagares;

            if (currentUserRole == "administrador")
            {
                pagares = await _pagareRepository.GetAllAsync();
            }
            else
            {
                pagares = await _pagareRepository.GetByIdUsuarioAsync(currentUserId);
            }

            return pagares.Select(MapToDto);
        }

        public async Task<PagareResponseDto> CreateAsync(PagareCreateDto dto, int idUsuario)
        {
            var pagare = new Pagare
            {
                NumeroExpediente = dto.NumeroExpediente,
                Monto = dto.Monto,
                PromesaPago = dto.PromesaPago,
                Beneficiario = dto.Beneficiario,
                FechaVencimiento = dto.FechaVencimiento,
                LugarPago = dto.LugarPago,
                FechaElaboracion = dto.FechaElaboracion,
                LugarSuscripcion = dto.LugarSuscripcion,
                Firma = dto.Firma,
                IdUsuario = idUsuario,
                FechaCreacion = DateTime.UtcNow
            };

            var created = await _pagareRepository.CreateAsync(pagare);
           await _auditoriaService.RegistrarAsync(created.IdPagare, idUsuario, "CREACION");

            return MapToDto(created);
        }

        public async Task<PagareResponseDto?> UpdateAsync(int id, PagareUpdateDto dto)
        {
            var pagare = await _pagareRepository.GetByIdAsync(id);
            if (pagare == null) return null;

            pagare.NumeroExpediente = dto.NumeroExpediente;
            pagare.Monto = dto.Monto;
            pagare.PromesaPago = dto.PromesaPago;
            pagare.Beneficiario = dto.Beneficiario;
            pagare.FechaVencimiento = dto.FechaVencimiento;
            pagare.LugarPago = dto.LugarPago;
            pagare.FechaElaboracion = dto.FechaElaboracion;
            pagare.LugarSuscripcion = dto.LugarSuscripcion;
            pagare.Firma = dto.Firma;

            var updated = await _pagareRepository.UpdateAsync(pagare);
            if (updated == null) return null;
            
            await _auditoriaService.RegistrarAsync(id, pagare.IdUsuario, "MODIFICACION");

            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pagare = await _pagareRepository.GetByIdAsync(id);
            if (pagare == null) return false;

            var result = await _pagareRepository.DeleteAsync(id);
            if (result)
            {
                await _auditoriaService.RegistrarAsync(id, pagare.IdUsuario, "MODIFICACION");
            }
            return result;
        }

        private PagareResponseDto MapToDto(Pagare pagare)
        {
            return new PagareResponseDto
            {
                IdPagare = pagare.IdPagare,
                NumeroExpediente = pagare.NumeroExpediente,
                Monto = pagare.Monto,
                PromesaPago = pagare.PromesaPago,
                Beneficiario = pagare.Beneficiario,
                FechaVencimiento = pagare.FechaVencimiento,
                LugarPago = pagare.LugarPago,
                FechaElaboracion = pagare.FechaElaboracion,
                LugarSuscripcion = pagare.LugarSuscripcion,
                Firma = pagare.Firma,
                IdUsuario = pagare.IdUsuario,
                NombreUsuario = pagare.Usuario?.Nombre ?? string.Empty,
                FechaCreacion = pagare.FechaCreacion
            };
        }
    }
}
