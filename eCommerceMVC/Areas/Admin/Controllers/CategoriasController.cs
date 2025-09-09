using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return View(categorias); // Index.cshtml sigue igual
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _categoriaService.GetByIdAsync(id.Value);
            if (categoria == null) return NotFound();

            return View(categoria); // Details.cshtml
        }

        // GET: Categorias/Create
        public IActionResult Create() => View(); // Create.cshtml

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            categoria.FechaRegistro = DateTime.Now;
            categoria.Activo = true;

            var result = await _categoriaService.CreateAsync(categoria);

            if (!result)
            {
                TempData["Error"] = "Ya existe una categoría con esa descripción.";
                return View(categoria);
            }

            TempData["Success"] = "Categoría creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _categoriaService.GetByIdAsync(id.Value);
            if (categoria == null) return NotFound();

            return View(categoria); // Edit.cshtml
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.IdCategoria) return NotFound();
            if (!ModelState.IsValid) return View(categoria);

            var result = await _categoriaService.UpdateAsync(categoria);

            if (!result)
            {
                TempData["Error"] = "Ya existe otra categoría con esa descripción.";
                return View(categoria);
            }

            TempData["Success"] = "Categoría actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _categoriaService.GetByIdAsync(id.Value);
            if (categoria == null) return NotFound();

            return View(categoria); // Delete.cshtml
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoriaService.DeleteAsync(id);

            if (!result)
                TempData["Error"] = "No se puede eliminar esta categoría porque tiene productos asociados.";
            else
                TempData["Success"] = "La categoría se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
