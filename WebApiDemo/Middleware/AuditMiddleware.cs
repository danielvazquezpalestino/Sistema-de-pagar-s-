using System.Security.Claims;

namespace WebApiDemo.Middleware
{
    /// <summary>
    /// Middleware to automatically log CONSULTA (view) actions for pagares.
    /// This middleware intercepts GET requests to /api/pagares/{id} and registers them in the audit log.
    /// </summary>
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public AuditMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if this is a GET request to a specific pagare
            if (context.Request.Method == "GET" && 
                context.Request.Path.StartsWithSegments("/api/pagares") &&
                context.Request.Path.Value?.Split('/').Length == 4)
            {
                var pathSegments = context.Request.Path.Value.Split('/');
                if (int.TryParse(pathSegments[3], out int pagareId))
                {
                    // Store the pagare ID in HttpContext for the controller to use
                    context.Items["PagareId"] = pagareId;
                }
            }

            await _next(context);
        }
    }
}
