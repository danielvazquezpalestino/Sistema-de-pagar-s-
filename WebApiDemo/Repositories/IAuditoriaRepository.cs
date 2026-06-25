using WebApiDemo.Models;

namespace WebApiDemo.Repositories
{
    public interface IAuditoriaRepository
    {
        Task<IEnumerable<Auditoria>> GetAllAsync();
        Task<IEnumerable<Auditoria>> GetByIdPagareAsync(int idPagare);
        Task<IEnumerable<Auditoria>> GetByIdUsuarioAsync(int idUsuario);
        Task<IEnumerable<Auditoria>> GetByAccionAsync(string accion);
        Task<Auditoria> CreateAsync(Auditoria auditoria);
    }
}
