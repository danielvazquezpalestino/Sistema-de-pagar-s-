using WebApiDemo.DTOs.Pagares;

namespace WebApiDemo.Services
{
    public interface IPagareService
    {
        Task<PagareResponseDto?> GetByIdAsync(int id, int currentUserId, string currentUserRole);
        Task<IEnumerable<PagareResponseDto>> GetAllAsync(int currentUserId, string currentUserRole);
        Task<PagareResponseDto> CreateAsync(PagareCreateDto dto, int idUsuario);
        Task<PagareResponseDto?> UpdateAsync(int id, PagareUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
