using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using WebApiDemo.Models;

namespace WebApiDemo.Repositories
{
    public class PagareRepository : IPagareRepository
    {
        private readonly AppDbContext _context;

        public PagareRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pagare?> GetByIdAsync(int id)
        {
            return await _context.Pagares.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.IdPagare == id);
        }

        public async Task<IEnumerable<Pagare>> GetAllAsync()
        {
            return await _context.Pagares.Include(p => p.Usuario).ToListAsync();
        }

        public async Task<IEnumerable<Pagare>> GetByIdUsuarioAsync(int idUsuario)
        {
            return await _context.Pagares.Include(p => p.Usuario).Where(p => p.IdUsuario == idUsuario).ToListAsync();
        }

        public async Task<Pagare> CreateAsync(Pagare pagare)
        {
            _context.Pagares.Add(pagare);
            await _context.SaveChangesAsync();
            return pagare;
        }

        public async Task<Pagare?> UpdateAsync(Pagare pagare)
        {
            _context.Pagares.Update(pagare);
            await _context.SaveChangesAsync();
            return pagare;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pagare = await _context.Pagares.FindAsync(id);
            if (pagare == null) return false;

            _context.Pagares.Remove(pagare);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
