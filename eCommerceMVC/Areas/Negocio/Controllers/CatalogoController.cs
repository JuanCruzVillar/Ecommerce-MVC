using eCommerce.Areas.Negocio.Controllers;
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Area("Negocio")]
public class CatalogoController : BaseNegocioController
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

        // Obtener ofertas usando el servicio
        var ahora = DateTime.Now;
        var ofertasService = HttpContext.RequestServices.GetService<IOfertaService>();
        var ofertas = new Dictionary<int, Oferta>();

        if (ofertasService != null)
        {
            foreach (var producto in productos)
            {
                var oferta = await ofertasService.ObtenerOfertaVigentePorProducto(producto.IdProducto);
                if (oferta != null)
                {
                    ofertas[producto.IdProducto] = oferta;
                }
            }
        }

        var viewModels = productos.Select(p => new DetalleProductoViewModel
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre ?? "Sin nombre",
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            RutaImagen = p.RutaImagen,
            IdCategoria = p.IdCategoria,
            PrecioOferta = ofertas.ContainsKey(p.IdProducto) ? ofertas[p.IdProducto].PrecioOferta : null,
            PorcentajeDescuento = ofertas.ContainsKey(p.IdProducto) ? ofertas[p.IdProducto].PorcentajeDescuento : null
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
                       (p.Nombre != null && p.Nombre.ToLower().Contains(termino.ToLower()) ||
                        (p.Descripcion != null && p.Descripcion.ToLower().Contains(termino.ToLower()))))
                .Take(5)
                .Select(p => new
                {
                    id = p.IdProducto,
                    nombre = p.Nombre ?? "Sin nombre",
                    precio = p.Precio?.ToString("N2") ?? "0.00",
                    imagen = p.RutaImagen ?? ""
                })
                .ToList();

            return Json(new { success = true, resultados });
        }
        catch (Exception ex)
        {
            // Log del error para debugging
            Console.WriteLine($"Error en BuscarAjax: {ex.Message}");
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

        // Mapear imagenes
        var imagenes = producto.Imagenes
     .OrderByDescending(i => i.EsPrincipal ?? false) // Principal primero
     .ThenBy(i => i.Orden) // Luego por orden
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