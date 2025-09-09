using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("Negocio")]
public class CatalogoController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;

    public CatalogoController(IProductoService productoService, ICategoriaService categoriaService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index(int? categoriaId = null)
    {
        var productos = await _productoService.GetAllAsync();

        // Filtrar por categoría si viene categoriaId
        if (categoriaId.HasValue)
        {
            productos = productos.Where(p => p.IdCategoria == categoriaId.Value).ToList();
        }

        // Traer todas las categorías para el sidebar
        var categorias = await _categoriaService.GetAllAsync();
        ViewBag.Categorias = categorias;

        // Mapeo de entidad a ViewModel
        var viewModels = productos.Select(p => new DetalleProductoViewModel
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre ?? "Sin nombre",
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            RutaImagen = p.RutaImagen,
            IdCategoria = p.IdCategoria
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
            RutaImagen = producto.RutaImagen,
            IdCategoria = producto.IdCategoria
        };

        // Productos relacionados (excluyendo el actual)
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
