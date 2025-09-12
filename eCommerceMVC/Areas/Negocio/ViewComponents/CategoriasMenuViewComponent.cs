using Microsoft.AspNetCore.Mvc;
using eCommerce.Services.Interfaces;
using System.Threading.Tasks;

namespace eCommerceMVC.ViewComponents
{
    public class CategoriasMenuViewComponent : ViewComponent
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasMenuViewComponent(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return View(categorias);
        }
    }
}
