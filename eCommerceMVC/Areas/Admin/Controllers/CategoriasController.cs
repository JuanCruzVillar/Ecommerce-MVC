using eCommerce.Areas.Admin.Controllers;
using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    public class CategoriasController : BaseAdminController
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            try
            {
                var categorias = await _categoriaService.GetAllAsync();
                return View(categorias);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las categorías. Intente nuevamente.";
                return View(new List<Categoria>());
            }
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var categoria = await _categoriaService.GetByIdAsync(id.Value);
                if (categoria == null) return NotFound();

                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los detalles de la categoría.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Categorias/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await CargarCategoriasActivas();
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                {
                    ModelState.AddModelError("Descripcion", "La descripción es obligatoria");
                }

                if (categoria.IdCategoriaPadre.HasValue && await ValidarJerarquiaCircular(categoria.IdCategoria, categoria.IdCategoriaPadre.Value))
                {
                    ModelState.AddModelError("IdCategoriaPadre", "No se puede crear una jerarquía circular");
                }

                if (!ModelState.IsValid)
                {
                    await CargarCategoriasActivas();
                    return View(categoria);
                }

                categoria.Descripcion = categoria.Descripcion.Trim();
                categoria.FechaRegistro = DateTime.Now;
                categoria.Activo = true;

                var result = await _categoriaService.CreateAsync(categoria);
                if (!result)
                {
                    TempData["Error"] = "Ya existe una categoría con esa descripción.";
                    await CargarCategoriasActivas();
                    return View(categoria);
                }

                TempData["Success"] = categoria.IdCategoriaPadre.HasValue
                    ? "Subcategoría creada correctamente."
                    : "Categoría creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error interno al crear la categoría.";
                await CargarCategoriasActivas();
                return View(categoria);
            }
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var categoria = await _categoriaService.GetByIdAsync(id.Value);
                if (categoria == null) return NotFound();

                await CargarCategoriasActivas(categoria.IdCategoria, categoria.IdCategoriaPadre);
                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la categoría para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Categorias Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.IdCategoria) return NotFound();

            try
            {
                if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                {
                    ModelState.AddModelError("Descripcion", "La descripción es obligatoria");
                }

                if (categoria.IdCategoriaPadre.HasValue && await ValidarJerarquiaCircular(categoria.IdCategoria, categoria.IdCategoriaPadre.Value))
                {
                    ModelState.AddModelError("IdCategoriaPadre", "No se puede crear una jerarquía circular");
                }

                if (!ModelState.IsValid)
                {
                    await CargarCategoriasActivas(categoria.IdCategoria, categoria.IdCategoriaPadre);
                    return View(categoria);
                }

                categoria.Descripcion = categoria.Descripcion.Trim();

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
            catch (Exception ex)
            {
                TempData["Error"] = "Error interno al actualizar la categoría.";
                await CargarCategoriasActivas(categoria.IdCategoria, categoria.IdCategoriaPadre);
                return View(categoria);
            }
        }

        // POST: Categorias Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var categoria = await _categoriaService.GetByIdAsync(id);

                if (categoria == null)
                {
                    TempData["Error"] = "La categoría no existe.";
                    return RedirectToAction(nameof(Index));
                }

                if (categoria.SubCategorias != null && categoria.SubCategorias.Any())
                {
                    TempData["Error"] = "No se puede eliminar esta categoría porque tiene subcategorías asociadas.";
                    return RedirectToAction(nameof(Index));
                }

                if (categoria.Productos != null && categoria.Productos.Any())
                {
                    TempData["Error"] = "No se puede eliminar esta categoría porque tiene productos asociados.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _categoriaService.DeleteAsync(id);

                if (result)
                {
                    TempData["Success"] = "La categoría se eliminó correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar la categoría. Intente nuevamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error interno al eliminar la categoría.";
            }

            return RedirectToAction(nameof(Index));
        }

        #region Metodos Auxiliares

        private async Task<bool> ValidarJerarquiaCircular(int categoriaId, int categoriaPadreId)
        {
            try
            {
                if (categoriaId == categoriaPadreId)
                    return true;

                var categoriaPadre = await _categoriaService.GetByIdAsync(categoriaPadreId);
                while (categoriaPadre?.IdCategoriaPadre.HasValue == true)
                {
                    if (categoriaPadre.IdCategoriaPadre.Value == categoriaId)
                        return true;

                    categoriaPadre = await _categoriaService.GetByIdAsync(categoriaPadre.IdCategoriaPadre.Value);
                }

                return false;
            }
            catch
            {
                return true;
            }
        }

        private async Task CargarCategoriasActivas(int? idExcluido = null, int? selectedId = null)
        {
            try
            {
                var categorias = await _categoriaService.GetCategoriasPrincipalesAsync();
                var lista = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "",
                        Text = "-- Categoría Principal --",
                        Selected = !selectedId.HasValue
                    }
                };

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

                    if (cat.SubCategorias != null && cat.SubCategorias.Any())
                    {
                        foreach (var sub in cat.SubCategorias)
                        {
                            if (idExcluido.HasValue && sub.IdCategoria == idExcluido.Value)
                                continue;

                            lista.Add(new SelectListItem
                            {
                                Value = sub.IdCategoria.ToString(),
                                Text = $"└── {sub.Descripcion}",
                                Selected = (selectedId.HasValue && sub.IdCategoria == selectedId.Value)
                            });
                        }
                    }
                }

                ViewBag.Categorias = lista;
            }
            catch (Exception ex)
            {
                ViewBag.Categorias = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "-- Error al cargar categorías --" }
                };
            }
        }

        #endregion

        #region API Endpoints para AJAX

        [HttpGet]
        public async Task<IActionResult> GetSubCategorias(int categoriaPadreId)
        {
            try
            {
                var categoria = await _categoriaService.GetByIdAsync(categoriaPadreId);
                if (categoria?.SubCategorias == null)
                {
                    return Json(new List<object>());
                }

                var subcategorias = categoria.SubCategorias
                    .Where(sc => sc.Activo)
                    .Select(sc => new {
                        idCategoria = sc.IdCategoria,
                        descripcion = sc.Descripcion
                    })
                    .ToList();

                return Json(subcategorias);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        #endregion
    }
}