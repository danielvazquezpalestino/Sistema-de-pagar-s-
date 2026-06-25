using System.Security.Claims;
using WebApiDemo.DTOs.Auth;

namespace WebApiDemo.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequest request, HttpContext httpContext);
        Task LogoutAsync(HttpContext httpContext);
        MeResponse? GetCurrentUser(ClaimsPrincipal user);
    }
}
