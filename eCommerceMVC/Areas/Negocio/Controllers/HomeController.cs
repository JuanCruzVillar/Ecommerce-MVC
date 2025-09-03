using Microsoft.AspNetCore.Mvc;
using eCommerce.Services.Interfaces;
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;

        public HomeController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.GetAllAsync();

            
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
    }
}
