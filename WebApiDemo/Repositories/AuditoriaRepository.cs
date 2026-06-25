using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using WebApiDemo.Models;

namespace WebApiDemo.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly AppDbContext _context;

        public AuditoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Auditoria>> GetAllAsync()
        {
            return await _context.Auditorias
                .Include(a => a.Pagare)
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> GetByIdPagareAsync(int idPagare)
        {
            return await _context.Auditorias
                .Include(a => a.Pagare)
                .Include(a => a.Usuario)
                .Where(a => a.IdPagare == idPagare)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> GetByIdUsuarioAsync(int idUsuario)
        {
            return await _context.Auditorias
                .Include(a => a.Pagare)
                .Include(a => a.Usuario)
                .Where(a => a.IdUsuario == idUsuario)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> GetByAccionAsync(string accion)
        {
            return await _context.Auditorias
                .Include(a => a.Pagare)
                .Include(a => a.Usuario)
                .Where(a => a.Accion == accion)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<Auditoria> CreateAsync(Auditoria auditoria)
        {
            _context.Auditorias.Add(auditoria);
            await _context.SaveChangesAsync();
            return auditoria;
        }
    }
}
