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
    public async Task<IActionResult> Index(int? categoria = null, string busqueda = null)
    {
        var productos = await _productoService.GetAllAsync();

        // Filtro por búsqueda
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            productos = productos.Where(p =>
                p.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                (p.Descripcion != null && p.Descripcion.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        if (categoria.HasValue)
        {
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


    [HttpGet]
    public async Task<IActionResult> BuscarAjax(string termino)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
            {
                return Json(new { success = false, message = "Mínimo 2 caracteres" });
            }

            var productos = await _productoService.GetAllAsync();
            var resultados = productos
                .Where(p => p.Activo == true &&
                       (p.Nombre.ToLower().Contains(termino.ToLower()) ||
                        p.Descripcion.ToLower().Contains(termino.ToLower())))
                .Take(5)
                .Select(p => new
                {
                    id = p.IdProducto,
                    nombre = p.Nombre,
                    precio = p.Precio?.ToString("N2"),
                    imagen = p.RutaImagen
                })
                .ToList();

            return Json(new { success = true, resultados });
        }
        catch
        {
            return Json(new { success = false, message = "Error en la búsqueda" });
        }
    }

    public async Task<IActionResult> Detalle(int id)
    {
        // Usar el nuevo método que incluye imágenes
        var producto = await _productoService.GetByIdWithImagenesAsync(id);

        if (producto == null)
            return NotFound();

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
            .Take(4)
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

        // Mapear especificaciones
        var especificaciones = producto.Especificaciones
            .OrderBy(e => e.Orden)
            .Select(e => new ProductoEspecificacionViewModel
            {
                Clave = e.Clave,
                Valor = e.Valor,
                Orden = e.Orden ?? 0
            })
            .ToList();

        // Mapear imágenes
        var imagenes = producto.Imagenes
            .OrderBy(i => i.Orden)
            .Select(i => new ProductoImagenViewModel
            {
                IdImagen = i.IdImagen,
                RutaImagen = i.RutaImagen,
                EsPrincipal = i.EsPrincipal ?? false,
                Orden = i.Orden ?? 0
            })
            .ToList();

        var viewModel = new DetalleProductoPaginaViewModel
        {
            Producto = productoVM,
            Relacionados = relacionados,
            Especificaciones = especificaciones,
            Imagenes = imagenes
        };

        return View(viewModel);
    }


}