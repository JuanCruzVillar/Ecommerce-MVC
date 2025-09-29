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

    // Lista de productos
    public async Task<IActionResult> Index(int? categoria = null)
    {
        var productos = await _productoService.GetAllAsync();

        if (categoria.HasValue)
        {
            // Filtrar productos por categoría y  subcategorías
            var categorias = await _categoriaService.GetAllAsync();
            var categoriasHijas = categorias
                .Where(c => c.IdCategoriaPadre == categoria.Value)
                .Select(c => c.IdCategoria)
                .ToList();

            categoriasHijas.Add(categoria.Value); 

            productos = productos.Where(p => categoriasHijas.Contains(p.IdCategoria ?? 0)).ToList();
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

        if (categoria.HasValue)
        {
            var categoriaActual = (await _categoriaService.GetAllAsync())
                .FirstOrDefault(c => c.IdCategoria == categoria.Value);
            ViewBag.CategoriaNombre = categoriaActual?.Descripcion;
        }

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

        
        var todosProductos = await _productoService.GetAllAsync();
        var relacionados = todosProductos
            .Where(p => p.IdCategoria == producto.IdCategoria && p.IdProducto != producto.IdProducto)
            .Take(4) // Mostrar 4 productos relacionados
            .Select(p => new DetalleProductoViewModel
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre ?? "Sin nombre",
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                RutaImagen = p.RutaImagen,
                IdCategoria = p.IdCategoria
            })
            .ToList();

        // Crear el ViewModel de la página
        var viewModel = new DetalleProductoPaginaViewModel
        {
            Producto = productoVM,
            Relacionados = relacionados
        };

        return View(viewModel);
    }
}