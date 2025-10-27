using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace eCommerce.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class CarritoController : BaseNegocioController
    {
        private readonly ICarritoService _carritoService;
        private readonly IProductoService _productoService;
        private readonly ILogger<CarritoController> _logger;

        public CarritoController(ICarritoService carritoService, IProductoService productoService, ILogger<CarritoController> logger)
        {
            _carritoService = carritoService;
            _productoService = productoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                // Usuario no autenticado - mostrar vista vacía para manejar con localStorage
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

                // Si NO está autenticado, devolver success para que JS maneje con localStorage
                if (clienteId == 0)
                {
                    // Validar que el producto existe
                    var producto = await _productoService.GetByIdAsync(productoId);
                    if (producto == null || !producto.Activo.GetValueOrDefault())
                    {
                        return Json(new { success = false, message = "Producto no disponible" });
                    }

                    return Json(new
                    {
                        success = true,
                        message = "Producto agregado al carrito",
                        sinSesion = true // Flag para que JS use localStorage
                    });
                }

                // Usuario autenticado - agregar a BD
                await _carritoService.AgregarProductoAsync(clienteId, productoId, cantidad);
                return Json(new { success = true, sinSesion = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto {ProductoId} al carrito", productoId);
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
                    return Json(new { success = true, sinSesion = true });
                }

                await _carritoService.EliminarProductoAsync(clienteId, productoId);
                return Json(new { success = true, sinSesion = false });
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
                    return Json(new { success = true, sinSesion = true });
                }

                await _carritoService.VaciarCarritoAsync(clienteId);
                return Json(new { success = true, sinSesion = false });
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

        // NUEVO: Migrar carrito de localStorage a BD al iniciar sesión
        [HttpPost]
        public async Task<IActionResult> MigrarCarrito([FromBody] List<CarritoLocalStorageItem> items)
        {
            try
            {
                var clienteId = GetClienteId();
                if (clienteId == 0)
                {
                    return Json(new { success = false, message = "Debe iniciar sesión" });
                }

                foreach (var item in items)
                {
                    await _carritoService.AgregarProductoAsync(clienteId, item.IdProducto, item.Cantidad);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> ObtenerProductosInfo([FromBody] List<int> productosIds)
        {
            try
            {
                var productos = new List<object>();

                foreach (var id in productosIds)
                {
                    var producto = await _productoService.GetByIdAsync(id);
                    if (producto != null && producto.Activo.GetValueOrDefault())
                    {
                        productos.Add(new
                        {
                            idProducto = producto.IdProducto,
                            nombre = producto.Nombre,
                            precio = producto.Precio,
                            rutaImagen = producto.RutaImagen,
                            stock = producto.Stock
                        });
                    }
                }

                return Json(new { success = true, productos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // Modelo para migración
    public class CarritoLocalStorageItem
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }
}