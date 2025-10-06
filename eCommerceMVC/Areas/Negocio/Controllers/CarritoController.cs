using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Areas.Negocio.Controllers
{
    public class CarritoController : BaseNegocioController
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

                // 🔍 DEBUG: Agregar estos logs
                Console.WriteLine($"DEBUG Agregar - ClienteId obtenido: {clienteId}");
                Console.WriteLine($"DEBUG Agregar - ProductoId: {productoId}");
                Console.WriteLine($"DEBUG Agregar - Cantidad: {cantidad}");
                Console.WriteLine($"DEBUG Agregar - Usuario autenticado: {User.Identity.IsAuthenticated}");

                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"DEBUG Agregar - Claim: {claim.Type} = {claim.Value}");
                }

                if (clienteId == 0)
                {
                    return Json(new { success = false, message = "Debe iniciar sesión" });
                }

                await _carritoService.AgregarProductoAsync(clienteId, productoId, cantidad);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"DEBUG Agregar - Error: {ex.Message}");
                Console.WriteLine($"DEBUG Agregar - StackTrace: {ex.StackTrace}");
                Console.WriteLine($"DEBUG Agregar - InnerException: {ex.InnerException?.Message}");

                return Json(new { success = false, message = $"Error: {ex.Message}" });
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
    }
}