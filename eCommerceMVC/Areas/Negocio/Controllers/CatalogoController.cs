using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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

    // Lista de productos, opcionalmente filtrados por categoría
    public async Task<IActionResult> Index(int? categoriaId = null)
    {
        var productos = await _productoService.GetAllAsync();

        if (categoriaId.HasValue)
        {
            productos = productos.Where(p => p.IdCategoria == categoriaId.Value).ToList();
        }

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

    // Detalle de producto
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

        return View(productoVM);
    }
}
