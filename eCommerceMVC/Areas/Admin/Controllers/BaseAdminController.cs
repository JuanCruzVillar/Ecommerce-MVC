using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public abstract class BaseAdminController : Controller
    {
        protected int GetUsuarioId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var usuarioIdClaim = User.FindFirst("IdUsuario")?.Value;
                if (!string.IsNullOrEmpty(usuarioIdClaim) && int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    return usuarioId;
                }
            }
            return 0;
        }
    }
}