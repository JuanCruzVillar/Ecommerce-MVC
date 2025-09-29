using Microsoft.AspNetCore.Mvc;
using eCommerce.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;

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
            var todasCategorias = await _categoriaService.GetAllAsync();

            
            var categoriasPrincipales = todasCategorias
                .Where(c => c.IdCategoriaPadre == null || c.IdCategoriaPadre == 0)
                .ToList();

            
            foreach (var categoria in categoriasPrincipales)
            {
                categoria.SubCategorias = todasCategorias
                    .Where(c => c.IdCategoriaPadre == categoria.IdCategoria)
                    .ToList();
            }

            return View(categoriasPrincipales);
        }
    }
}