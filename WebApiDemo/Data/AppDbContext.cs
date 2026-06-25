using Microsoft.EntityFrameworkCore;
using WebApiDemo.Models;

namespace WebApiDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pagare> Pagares { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Respaldo> Respaldos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Usuario entity
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.Rol).HasConversion<string>();
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            // Configure Pagare entity
            modelBuilder.Entity<Pagare>(entity =>
            {
                entity.HasKey(e => e.IdPagare);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Pagares)
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Auditoria entity
            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.HasKey(e => e.IdAuditoria);
                entity.Property(e => e.Accion).HasConversion<string>();
                
                entity.HasOne(e => e.Pagare)
                      .WithMany(p => p.Auditorias)
                      .HasForeignKey(e => e.IdPagare)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Auditorias)
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Respaldo entity
            modelBuilder.Entity<Respaldo>(entity =>
            {
                entity.HasKey(e => e.IdRespaldo);
            });
        }
    }
}
