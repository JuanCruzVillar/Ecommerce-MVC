using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

[Area("Negocio")]
public class CatalogoController : Controller
{
    private readonly IProductoService _productoService;

    public CatalogoController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.GetAllAsync();

        // Mapeo de entidad a ViewModel (para listado)
        var viewModels = productos.Select(p => new DetalleProductoViewModel
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre ?? "Sin nombre",
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            RutaImagen = p.RutaImagen
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        var productoVM = new DetalleProductoViewModel
        {
            IdProducto = producto.IdProducto,
            Nombre = producto.Nombre ?? "Sin nombre",
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            RutaImagen = producto.RutaImagen
        };

        // Productos relacionados 
        var todos = await _productoService.GetAllAsync();
        var relacionados = todos
            .Where(p => p.IdProducto != id)
            .Take(4)
            .Select(p => new DetalleProductoViewModel
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre ?? "Sin nombre",
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                RutaImagen = p.RutaImagen
            }).ToList();

        var paginaVM = new DetalleProductoPaginaViewModel
        {
            Producto = productoVM,
            Relacionados = relacionados
        };

        return View(paginaVM);
    }
}
