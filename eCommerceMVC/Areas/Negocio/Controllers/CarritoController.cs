using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        int clienteId = 1; // temporal, despues usar usuario logueado
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
        int clienteId = 1;
        await _carritoService.AgregarProductoAsync(clienteId, productoId, cantidad);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int productoId)
    {
        int clienteId = 1;
        await _carritoService.EliminarProductoAsync(clienteId, productoId);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Vaciar()
    {
        int clienteId = 1;
        await _carritoService.VaciarCarritoAsync(clienteId);
        return Ok();
    }

    [HttpGet]
    public async Task<int> Cantidad()
    {
        int clienteId = 1;
        var carrito = await _carritoService.ObtenerCarritoAsync(clienteId);
        return carrito.Sum(c => c.Cantidad ?? 1);
    }
}
