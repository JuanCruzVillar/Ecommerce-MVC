using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
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
            return View(categorias);
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _categoriaService.GetByIdAsync(id.Value);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        // GET: Categorias/Create
        public async Task<IActionResult> Create()
        {
            await CargarCategoriasActivas();
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasActivas();
                return View(categoria);
            }

            categoria.FechaRegistro = DateTime.Now;
            categoria.Activo = true;

            var result = await _categoriaService.CreateAsync(categoria);
            if (!result)
            {
                TempData["Error"] = "Ya existe una categoría con esa descripción.";
                await CargarCategoriasActivas();
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

            await CargarCategoriasActivas(categoria.IdCategoria, categoria.IdCategoriaPadre);
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.IdCategoria) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCategoriasActivas(categoria.IdCategoria, categoria.IdCategoriaPadre);
                return View(categoria);
            }

            var result = await _categoriaService.UpdateAsync(categoria);
            if (!result)
            {
                TempData["Error"] = "Ya existe otra categoría con esa descripción.";
                await CargarCategoriasActivas(categoria.IdCategoria, categoria.IdCategoriaPadre);
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

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoriaService.DeleteAsync(id);

            if (!result)
                TempData["Error"] = "No se puede eliminar esta categoría porque tiene productos asociados o subcategorías.";
            else
                TempData["Success"] = "La categoría se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para cargar las categorías activas en ViewBag
        private async Task CargarCategoriasActivas(int? idExcluido = null, int? selectedId = null)
        {
            var categorias = await _categoriaService.GetCategoriasPrincipalesAsync();
            var lista = new List<SelectListItem>();

            foreach (var cat in categorias)
            {
                if (idExcluido.HasValue && cat.IdCategoria == idExcluido.Value)
                    continue;

                lista.Add(new SelectListItem
                {
                    Value = cat.IdCategoria.ToString(),
                    Text = cat.Descripcion,
                    Selected = (selectedId.HasValue && cat.IdCategoria == selectedId.Value)
                });

                if (cat.SubCategorias != null)
                {
                    foreach (var sub in cat.SubCategorias)
                    {
                        lista.Add(new SelectListItem
                        {
                            Value = sub.IdCategoria.ToString(),
                            Text = "-- " + sub.Descripcion,
                            Selected = (selectedId.HasValue && sub.IdCategoria == selectedId.Value)
                        });
                    }
                }
            }

            ViewBag.Categorias = lista;
        }
    }
}
