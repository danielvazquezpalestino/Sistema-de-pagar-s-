namespace WebApiDemo.Services
{
    /// <summary>
    /// Singleton service for application-wide configuration.
    /// Registered as Singleton in DI because configuration data is read-only and shared across all requests.
    /// This ensures efficient memory usage and consistent configuration throughout the application lifecycle.
    /// </summary>
    public class AppConfigurationService
    {
        public string ApplicationName { get; set; } = "Pagares API";
        public string Version { get; set; } = "1.0.0";
        public int MaxPageSize { get; set; } = 50;
        public string[] AllowedRoles { get; set; } = new[] { "abogado", "administrador" };
    }
}
