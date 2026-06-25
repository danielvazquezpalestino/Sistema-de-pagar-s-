using WebApiDemo.Models;

namespace WebApiDemo.Repositories
{
    public interface IPagareRepository
    {
        Task<Pagare?> GetByIdAsync(int id);
        Task<IEnumerable<Pagare>> GetAllAsync();
        Task<IEnumerable<Pagare>> GetByIdUsuarioAsync(int idUsuario);
        Task<Pagare> CreateAsync(Pagare pagare);
        Task<Pagare?> UpdateAsync(Pagare pagare);
        Task<bool> DeleteAsync(int id);
    }
}
