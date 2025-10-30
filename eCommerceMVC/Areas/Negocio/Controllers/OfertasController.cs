using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class OfertasController : Controller
    {
        private readonly IOfertaService _ofertaService;

        public OfertasController(IOfertaService ofertaService)
        {
            _ofertaService = ofertaService;
        }

        public async Task<IActionResult> Index()
        {
            var productosEnOferta = await _ofertaService.ObtenerProductosEnOferta();

            var viewModel = new OfertasViewModel
            {
                ProductosEnOferta = productosEnOferta,
                TotalOfertas = productosEnOferta.Count,
                TituloSeccion = "🔥 Ofertas Especiales",
                DescripcionSeccion = $"Encontramos {productosEnOferta.Count} productos en oferta para vos"
            };

            return View(viewModel);
        }
    }
}