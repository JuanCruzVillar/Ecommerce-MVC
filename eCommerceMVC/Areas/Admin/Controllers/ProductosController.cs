using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IMarcaService _marcaService;

        // MEJORADO: Configuración para archivos
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public ProductosController(
            IProductoService productoService,
            ICategoriaService categoriaService,
            IMarcaService marcaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _marcaService = marcaService;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            try
            {
                var productos = await _productoService.GetAllWithCategoriasAsync();
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los productos. Intente nuevamente.";
                
                return View(new List<Producto>());
            }
        }

        // GET: Productos/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await CargarDropdowns();
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel vm)
        {
            try
            {
                
                if (vm.ImagenArchivo != null)
                {
                    var validationResult = ValidateImage(vm.ImagenArchivo);
                    if (!string.IsNullOrEmpty(validationResult))
                    {
                        ModelState.AddModelError("ImagenArchivo", validationResult);
                    }
                }

                if (!ModelState.IsValid)
                {
                    await CargarDropdowns(vm);
                    return View(vm);
                }

                var producto = new Producto
                {
                    Nombre = vm.Nombre?.Trim(),
                    Descripcion = vm.Descripcion?.Trim(),
                    IdMarca = vm.IdMarca,
                    IdCategoria = vm.IdCategoria,
                    Precio = vm.Precio,
                    Stock = vm.Stock,
                    Activo = vm.Activo,
                    FechaRegistro = DateTime.Now
                };

                // MEJORADO: Procesamiento de imagen con validaciones
                if (vm.ImagenArchivo != null && vm.ImagenArchivo.Length > 0)
                {
                    var imagePath = await ProcessImageAsync(vm.ImagenArchivo);
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        producto.RutaImagen = imagePath;
                        producto.NombreImagen = vm.ImagenArchivo.FileName;
                    }
                }

                var success = await _productoService.CreateAsync(producto);
                if (success)
                {
                    TempData["Success"] = "Producto creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Error al crear el producto";
                    await CargarDropdowns(vm);
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error interno al crear el producto.";
                await CargarDropdowns(vm);
                return View(vm);
            }
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var producto = await _productoService.GetByIdAsync(id.Value);
                if (producto == null) return NotFound();

                var vm = new ProductoViewModel
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    IdMarca = producto.IdMarca,
                    IdCategoria = producto.IdCategoria,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    Activo = producto.Activo == true,
                    FechaRegistro = producto.FechaRegistro,
                    RutaImagen = producto.RutaImagen,
                    NombreImagen = producto.NombreImagen
                };

                await CargarDropdowns(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el producto para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel vm)
        {
            if (id != vm.IdProducto) return NotFound();

            try
            {
                
                if (vm.ImagenArchivo != null)
                {
                    var validationResult = ValidateImage(vm.ImagenArchivo);
                    if (!string.IsNullOrEmpty(validationResult))
                    {
                        ModelState.AddModelError("ImagenArchivo", validationResult);
                    }
                }

                if (!ModelState.IsValid)
                {
                    await CargarDropdowns(vm);
                    return View(vm);
                }

                var producto = await _productoService.GetByIdAsync(id);
                if (producto == null) return NotFound();

                // Actualizar campos
                producto.Nombre = vm.Nombre?.Trim();
                producto.Descripcion = vm.Descripcion?.Trim();
                producto.IdMarca = vm.IdMarca;
                producto.IdCategoria = vm.IdCategoria;
                producto.Precio = vm.Precio;
                producto.Stock = vm.Stock;
                producto.Activo = vm.Activo;

                
                if (vm.ImagenArchivo != null && vm.ImagenArchivo.Length > 0)
                {
                    var imagePath = await ProcessImageAsync(vm.ImagenArchivo);
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        // eliminar imagen anterior si existe
                        producto.RutaImagen = imagePath;
                        producto.NombreImagen = vm.ImagenArchivo.FileName;
                    }
                }

                var success = await _productoService.UpdateAsync(producto);
                if (success)
                {
                    TempData["Success"] = "Producto modificado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Error al modificar el producto";
                    await CargarDropdowns(vm);
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error interno al modificar el producto.";
                await CargarDropdowns(vm);
                return View(vm);
            }
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var producto = await _productoService.GetByIdAsync(id.Value);
                if (producto == null) return NotFound();

                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los detalles del producto.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var producto = await _productoService.GetByIdAsync(id.Value);
                if (producto == null) return NotFound();

                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el producto para eliminación.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _productoService.DeleteAsync(id);
                if (result)
                {
                    TempData["Success"] = "Producto eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el producto.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error interno al eliminar el producto.";
            }

            return RedirectToAction(nameof(Index));
        }

        #region Métodos Auxiliares

       
        private string ValidateImage(IFormFile file)
        {
            if (file == null) return null;

            // Validar tamaño
            if (file.Length > _maxFileSize)
            {
                return "El archivo no puede superar los 5MB.";
            }

            
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            {
                return "Solo se permiten archivos de imagen (.jpg, .jpeg, .png, .gif, .webp).";
            }

            return null; // Válido
        }


        private async Task<string> ProcessImageAsync(IFormFile file)
        {
            try
            {
                var nombreArchivo = Guid.NewGuid() + ".jpg"; // 🔹 Siempre JPG
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

                using var image = await Image.LoadAsync<Rgba32>(file.OpenReadStream());

                // 🔹 Redimensionar a 800x800 con fondo blanco
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(800, 800),
                    Mode = ResizeMode.Pad,
                    Position = AnchorPositionMode.Center,
                    PadColor = Color.White
                }));

                // 🔹 Guardar siempre como JPG optimizado
                var encoder = new JpegEncoder
                {
                    Quality = 85 // entre 70-90 es ideal: buena calidad y poco peso
                };

                await image.SaveAsync(rutaArchivo, encoder);

                return "/images/" + nombreArchivo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Cargar dropdowns de marcas y categorias/subcategorias
        private async Task CargarDropdowns(ProductoViewModel vm = null)
        {
            try
            {
                var marcas = await _marcaService.GetAllAsync();
                var categorias = await _categoriaService.GetCategoriasPrincipalesAsync();

                var listaCategorias = new List<SelectListItem>();
                foreach (var cat in categorias)
                {
                    listaCategorias.Add(new SelectListItem
                    {
                        Value = cat.IdCategoria.ToString(),
                        Text = cat.Descripcion,
                        Selected = vm?.IdCategoria == cat.IdCategoria
                    });

                    if (cat.SubCategorias != null && cat.SubCategorias.Any())
                    {
                        foreach (var sub in cat.SubCategorias)
                        {
                            listaCategorias.Add(new SelectListItem
                            {
                                Value = sub.IdCategoria.ToString(),
                                Text = "-- " + sub.Descripcion,
                                Selected = vm?.IdCategoria == sub.IdCategoria
                            });
                        }
                    }
                }

                ViewBag.IdMarca = new SelectList(marcas, "IdMarca", "Descripcion", vm?.IdMarca);
                ViewBag.IdCategoria = listaCategorias;
            }
            catch (Exception ex)
            {
                
                ViewBag.IdMarca = new SelectList(new List<object>(), "IdMarca", "Descripcion");
                ViewBag.IdCategoria = new List<SelectListItem>();
            }
        }

        #endregion
    }
}