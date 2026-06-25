namespace WebApiDemo.Services
{
    public interface IAuditoriaService
    {
        Task RegistrarAsync(int idPagare, int idUsuario, string accion);
    }
}
