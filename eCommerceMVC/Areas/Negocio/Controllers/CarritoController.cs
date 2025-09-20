using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Area("Negocio")]
public class CarritoController : Controller
{
    private readonly ICarritoService _carritoService;

    public CarritoController(ICarritoService carritoService)
    {
        _carritoService = carritoService;
    }

    public async Task<IActionResult> Index()
    {
        var clienteId = GetClienteId();
        if (clienteId == 0)
        {
            // Si no está logueado mostrar carrito vacío o redirigir al login
            return View(new List<CarritoViewModel>());
        }

        var carrito = await _carritoService.ObtenerCarritoAsync(clienteId);
        var model = carrito.Select(c => new CarritoViewModel
        {
            IdProducto = c.IdProducto.Value,
            Nombre = c.IdProductoNavigation?.Nombre ?? "",
            RutaImagen = c.IdProductoNavigation?.RutaImagen,
            Precio = c.IdProductoNavigation?.Precio,
            Cantidad = c.Cantidad ?? 1
        }).ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(int productoId, int cantidad = 1)
    {
        try
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                return Json(new { success = false, message = "Debe iniciar sesión" });
            }

            await _carritoService.AgregarProductoAsync(clienteId, productoId, cantidad);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error al agregar producto" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int productoId)
    {
        try
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                return Json(new { success = false, message = "Debe iniciar sesión" });
            }

            await _carritoService.EliminarProductoAsync(clienteId, productoId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error al eliminar producto" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Vaciar()
    {
        try
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                return Json(new { success = false, message = "Debe iniciar sesión" });
            }

            await _carritoService.VaciarCarritoAsync(clienteId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error al vaciar carrito" });
        }
    }

    [HttpGet]
    public async Task<int> Cantidad()
    {
        try
        {
            var clienteId = GetClienteId();
            if (clienteId == 0) return 0;

            
            return await _carritoService.ObtenerCantidadItemsAsync(clienteId);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    
    private int GetClienteId()
    {
        if (User.Identity.IsAuthenticated)
        {

            var clienteIdClaim = User.FindFirst("IdUsuario")?.Value;
            if (!string.IsNullOrEmpty(clienteIdClaim) && int.TryParse(clienteIdClaim, out int clienteId))
            {
                return clienteId;
            }
        }
        return 0;
    }

}