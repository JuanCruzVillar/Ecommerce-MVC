using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Negocio.ViewComponents
{
    public class CarritoResumenViewComponent : ViewComponent
    {
        private readonly ICarritoService _carritoService;
        public CarritoResumenViewComponent(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int clienteId = 1; 
            var carrito = await _carritoService.ObtenerCarritoAsync(clienteId);
            int totalProductos = 0;
            if (carrito != null)
                totalProductos = carrito.Sum(c => c.Cantidad ?? 0);

            return View(totalProductos);
        }
    }
}
