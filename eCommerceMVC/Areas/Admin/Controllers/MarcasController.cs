using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MarcasController : Controller
    {
        private readonly IMarcaService _marcaService;

        public MarcasController(IMarcaService marcaService)
        {
            _marcaService = marcaService;
        }
        // GET: Marcas
        public async Task<IActionResult> Index()
        {
            var marcas = await _marcaService.GetAllAsync();
            return View(marcas);
        }

        // GET: Marcas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var marca = await _marcaService.GetByIdAsync(id.Value);
            if (marca == null) return NotFound();

            return View(marca);
        }

        // GET: Marcas/Create
        public IActionResult Create() => View();

        // POST: Marcas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca marca)
        {
            if (!ModelState.IsValid) return View(marca);

            marca.FechaRegistro = DateTime.Now;
            marca.Activo = true;

            var result = await _marcaService.CreateAsync(marca);
            if (!result)
            {
                TempData["Error"] = "Ya existe una marca con esa descripción.";
                return View(marca);
            }

            TempData["Success"] = "Marca creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Marcas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var marca = await _marcaService.GetByIdAsync(id.Value);
            if (marca == null) return NotFound();

            return View(marca);
        }

        // POST: Marcas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marca marca)
        {
            if (id != marca.IdMarca) return NotFound();
            if (!ModelState.IsValid) return View(marca);

            var result = await _marcaService.UpdateAsync(marca);
            if (!result)
            {
                TempData["Error"] = "Ya existe otra marca con esa descripción.";
                return View(marca);
            }

            TempData["Success"] = "Marca actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Marcas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var marca = await _marcaService.GetByIdAsync(id.Value);
            if (marca == null) return NotFound();

            return View(marca);
        }

        // POST: Marcas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _marcaService.DeleteAsync(id);

            if (!result)
                TempData["Error"] = "No se puede eliminar esta marca porque tiene productos asociados.";
            else
                TempData["Success"] = "La marca se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
