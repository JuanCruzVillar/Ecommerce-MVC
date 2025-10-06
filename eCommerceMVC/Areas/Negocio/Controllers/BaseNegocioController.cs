using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public abstract class BaseNegocioController : Controller
    {
        protected int GetClienteId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                
                var clienteIdClaim = User.FindFirst("IdCliente")?.Value;
                if (!string.IsNullOrEmpty(clienteIdClaim) && int.TryParse(clienteIdClaim, out int clienteId))
                {
                    return clienteId;
                }
            }
            return 0;
        }

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