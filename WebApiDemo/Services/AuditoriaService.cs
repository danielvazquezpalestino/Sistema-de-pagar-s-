using WebApiDemo.Models;
using WebApiDemo.Repositories;
using WebApiDemo.Patterns;

namespace WebApiDemo.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;

        public AuditoriaService(IAuditoriaRepository auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task RegistrarAsync(int idPagare, int idUsuario, string accion)
        {
            try
            {
                var auditoria = new Auditoria
                {
                    IdPagare = idPagare,
                    IdUsuario = idUsuario,
                    Accion = accion,
                    Fecha = DateTime.UtcNow
                };

                await _auditoriaRepository.CreateAsync(auditoria);
            }
            catch (Exception ex)
            {
                // Log pero no lanzar - la auditoría es complementaria, no bloquea la operación
                SingletonLogger.Instance.Log($"Error al registrar auditoría: {ex.Message}");
            }
        }
    }
}
