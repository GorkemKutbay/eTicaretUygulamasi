using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class BaseController : Controller
    {
        protected int GetCurrentUserId()
        {
            // Hem 'sub' hem de 'NameIdentifier' (standart ASP.NET ID claim'i) kontrol edilir
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
        }

        protected bool IsUserRole(string role)
        {
            return User.IsInRole(role);
        }
    }
}
