using eCommerce.Areas.Negocio.Controllers;
using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class CheckoutController : BaseNegocioController
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICarritoService _carritoService;
        private readonly DbecommerceContext _context;

        public CheckoutController(ICheckoutService checkoutService, ICarritoService carritoService, DbecommerceContext context)
        {
            _checkoutService = checkoutService;
            _carritoService = carritoService;
            _context = context;
        }

        // GET: /Cliente/Checkout
        public async Task<IActionResult> Index()
        {
            try
            {
                var idCliente = GetClienteId();

                Console.WriteLine($"DEBUG - IdCliente obtenido: {idCliente}");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"DEBUG - Claim: {claim.Type} = {claim.Value}");
                }

                if (idCliente == 0)
                {
                    TempData["Error"] = "Debe iniciar sesión para continuar con la compra";
                    return RedirectToAction("Login", "Auth");
                }

                var itemsCarrito = await _carritoService.ObtenerItemsCarritoAsync(idCliente);
                Console.WriteLine($"DEBUG - Items carrito encontrados: {itemsCarrito.Count}");

                if (!itemsCarrito.Any())
                {
                    TempData["Error"] = "Su carrito está vacío";
                    return RedirectToAction("Index", "Catalogo");
                }

                var stockDisponible = await _checkoutService.ValidarStockProductosAsync(idCliente);
                if (!stockDisponible)
                {
                    TempData["Error"] = "Algunos productos no tienen stock suficiente. Revise su carrito.";
                    return RedirectToAction("Index", "Carrito");
                }

                var checkoutViewModel = await _checkoutService.ObtenerCheckoutAsync(idCliente);
                Console.WriteLine($"DEBUG - Direcciones disponibles: {checkoutViewModel.DireccionesDisponibles.Count}");

                if (!checkoutViewModel.DireccionesDisponibles.Any())
                {
                    checkoutViewModel.UsarNuevaDireccion = true;
                    Console.WriteLine("DEBUG - Forzando UsarNuevaDireccion = true");
                }

                return View(checkoutViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG - Error en Index: {ex.Message}");
                TempData["Error"] = "Error al cargar el checkout: " + ex.Message;
                return RedirectToAction("Index", "Carrito");
            }
        }

        // POST: Aplicar cupón
        [HttpPost]
        public async Task<IActionResult> AplicarCupon([FromBody] CuponViewModel cuponModel)
        {
            try
            {
                var idCliente = GetClienteId();
                var checkoutViewModel = await _checkoutService.ObtenerCheckoutAsync(idCliente);
                var subtotal = checkoutViewModel.Subtotal;

                var resultado = await _checkoutService.ValidarCuponAsync(cuponModel.Codigo, subtotal);

                if (resultado.EsValido)
                {
                    var costoEnvio = checkoutViewModel.CostoEnvio;
                    var total = subtotal + costoEnvio - resultado.DescuentoAplicado;

                    return Json(new
                    {
                        exito = true,
                        mensaje = resultado.Mensaje,
                        descuento = resultado.DescuentoAplicado,
                        nuevoTotal = total,
                        tipoDescuento = resultado.TipoDescuento
                    });
                }
                else
                {
                    return Json(new
                    {
                        exito = false,
                        mensaje = resultado.Mensaje
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = "Error al validar el cupón: " + ex.Message
                });
            }
        }

        // POST: Agregar nueva dirección
        [HttpPost]
        public async Task<IActionResult> AgregarDireccion([FromBody] DireccionEnvioViewModel direccionModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new
                    {
                        exito = false,
                        mensaje = "Datos inválidos: " + string.Join(", ", errores)
                    });
                }

                var idCliente = GetClienteId();
                var resultado = await _checkoutService.AgregarDireccionEnvioAsync(direccionModel, idCliente);

                if (resultado)
                {
                    var checkoutViewModel = await _checkoutService.ObtenerCheckoutAsync(idCliente);
                    var direccionesHtml = await RenderPartialViewToStringAsync("_DireccionesEnvioPartial", checkoutViewModel.DireccionesDisponibles);

                    return Json(new
                    {
                        exito = true,
                        mensaje = "Dirección agregada exitosamente",
                        direccionesHtml = direccionesHtml
                    });
                }
                else
                {
                    return Json(new
                    {
                        exito = false,
                        mensaje = "Error al agregar la dirección"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = "Error: " + ex.Message
                });
            }
        }

        // POST: Calcular costo de envío
        [HttpPost]
        public async Task<IActionResult> CalcularEnvio([FromBody] int idDireccionEnvio)
        {
            try
            {
                var costoEnvio = await _checkoutService.CalcularCostoEnvioAsync(idDireccionEnvio);

                return Json(new
                {
                    exito = true,
                    costoEnvio = costoEnvio
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = "Error al calcular envío: " + ex.Message
                });
            }
        }

        // POST: Procesar pedido final
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPedido(CheckoutViewModel model)
        {
            var idCliente = GetClienteId();

            try
            {
                // Limpiar ModelState de campos que no necesitan validación
                ModelState.Remove("Cliente");
                ModelState.Remove("ItemsCarrito");
                ModelState.Remove("DireccionesDisponibles");
                ModelState.Remove("MetodosPagoDisponibles");
                ModelState.Remove("CodigoCupon");
                ModelState.Remove("MensajeCupon");
                ModelState.Remove("NotasEspeciales");

                Console.WriteLine($"DEBUG - ProcesarPedido IdCliente: {idCliente}");
                Console.WriteLine($"DEBUG - MetodoPago: {model.MetodoPagoSeleccionado}");
                Console.WriteLine($"DEBUG - UsarNuevaDireccion: {model.UsarNuevaDireccion}");

                // Validación de método de pago
                if (model.MetodoPagoSeleccionado == 0)
                {
                    Console.WriteLine("DEBUG - Error: Método de pago no seleccionado");
                    ModelState.AddModelError("MetodoPagoSeleccionado", "Debe seleccionar un método de pago");
                    return await RecargarCheckoutConError(model, idCliente);
                }

                // Si usa nueva dirección, validarla y guardarla
                if (model.UsarNuevaDireccion)
                {
                    Console.WriteLine("DEBUG - Validando nueva dirección...");

                    if (model.NuevaDireccion == null)
                    {
                        Console.WriteLine("DEBUG - Error: NuevaDireccion es null");
                        ModelState.AddModelError("", "Los datos de la dirección son requeridos");
                        return await RecargarCheckoutConError(model, idCliente);
                    }

                    // Validar campos requeridos
                    var erroresDireccion = new List<string>();

                    if (string.IsNullOrWhiteSpace(model.NuevaDireccion.NombreCompleto))
                        erroresDireccion.Add("El nombre completo es requerido");
                    if (string.IsNullOrWhiteSpace(model.NuevaDireccion.Direccion))
                        erroresDireccion.Add("La dirección es requerida");
                    if (string.IsNullOrWhiteSpace(model.NuevaDireccion.Ciudad))
                        erroresDireccion.Add("La ciudad es requerida");
                    if (string.IsNullOrWhiteSpace(model.NuevaDireccion.Provincia))
                        erroresDireccion.Add("La provincia es requerida");
                    if (string.IsNullOrWhiteSpace(model.NuevaDireccion.CodigoPostal))
                        erroresDireccion.Add("El código postal es requerido");
                    if (string.IsNullOrWhiteSpace(model.NuevaDireccion.Telefono))
                        erroresDireccion.Add("El teléfono es requerido");

                    if (erroresDireccion.Any())
                    {
                        Console.WriteLine($"DEBUG - Errores en dirección: {string.Join(", ", erroresDireccion)}");
                        foreach (var error in erroresDireccion)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return await RecargarCheckoutConError(model, idCliente);
                    }

                    // Crear la entidad DireccionEnvio directamente
                    var nuevaDireccion = new DireccionEnvio
                    {
                        IdCliente = idCliente,
                        NombreCompleto = model.NuevaDireccion.NombreCompleto.Trim(),
                        Direccion = model.NuevaDireccion.Direccion.Trim(),
                        Ciudad = model.NuevaDireccion.Ciudad.Trim(),
                        Provincia = model.NuevaDireccion.Provincia.Trim(),
                        CodigoPostal = model.NuevaDireccion.CodigoPostal.Trim(),
                        Telefono = model.NuevaDireccion.Telefono.Trim(),
                        Referencias = model.NuevaDireccion.Referencias?.Trim(),
                        EsDireccionPrincipal = model.NuevaDireccion.EsDireccionPrincipal,
                        FechaRegistro = DateTime.Now,
                        Activo = true
                    };

                    _context.DireccionesEnvio.Add(nuevaDireccion);
                    await _context.SaveChangesAsync();

                    model.DireccionEnvioSeleccionada = nuevaDireccion.IdDireccionEnvio;
                    Console.WriteLine($"DEBUG - Nueva dirección guardada con ID: {nuevaDireccion.IdDireccionEnvio}");
                }
                else
                {
                    // Validar que se haya seleccionado una dirección existente
                    if (!model.DireccionEnvioSeleccionada.HasValue || model.DireccionEnvioSeleccionada == 0)
                    {
                        Console.WriteLine("DEBUG - Error: Dirección no seleccionada");
                        ModelState.AddModelError("DireccionEnvioSeleccionada", "Debe seleccionar una dirección de envío");
                        return await RecargarCheckoutConError(model, idCliente);
                    }

                    // Verificar que la dirección pertenece al cliente
                    var direccionExiste = await _context.DireccionesEnvio
                        .AnyAsync(d => d.IdDireccionEnvio == model.DireccionEnvioSeleccionada.Value
                                    && d.IdCliente == idCliente
                                    && d.Activo);

                    if (!direccionExiste)
                    {
                        ModelState.AddModelError("", "La dirección seleccionada no es válida");
                        return await RecargarCheckoutConError(model, idCliente);
                    }
                }

                // Validar términos
                if (!model.AceptaTerminos)
                {
                    Console.WriteLine("DEBUG - Error: Términos no aceptados");
                    ModelState.AddModelError("AceptaTerminos", "Debe aceptar los términos y condiciones");
                    return await RecargarCheckoutConError(model, idCliente);
                }

                Console.WriteLine("DEBUG - Todas las validaciones pasaron, procesando pedido...");

                // Validar stock
                var stockDisponible = await _checkoutService.ValidarStockProductosAsync(idCliente);
                if (!stockDisponible)
                {
                    ModelState.AddModelError("", "Algunos productos ya no tienen stock disponible");
                    return await RecargarCheckoutConError(model, idCliente);
                }

                // Obtener datos actualizados del carrito
                var checkoutActualizado = await _checkoutService.ObtenerCheckoutAsync(idCliente);

                // Aplicar cupón si existe
                if (!string.IsNullOrEmpty(model.CodigoCupon))
                {
                    var resultadoCupon = await _checkoutService.ValidarCuponAsync(model.CodigoCupon, checkoutActualizado.Subtotal);
                    if (resultadoCupon.EsValido)
                    {
                        model.DescuentoAplicado = resultadoCupon.DescuentoAplicado;
                        model.CuponAplicado = true;
                    }
                }

                // Actualizar datos del modelo
                model.ItemsCarrito = checkoutActualizado.ItemsCarrito;
                model.Subtotal = checkoutActualizado.Subtotal;
                model.TotalItems = checkoutActualizado.TotalItems;

                // Calcular costo de envío
                if (model.DireccionEnvioSeleccionada.HasValue)
                {
                    model.CostoEnvio = await _checkoutService.CalcularCostoEnvioAsync(model.DireccionEnvioSeleccionada.Value);
                }

                model.Total = model.Subtotal + model.CostoEnvio - model.DescuentoAplicado;

                // Procesar el pedido
                var idVenta = await _checkoutService.ProcesarPedidoAsync(model, idCliente);

                if (idVenta > 0)
                {
                    Console.WriteLine($"DEBUG - Pedido procesado exitosamente, ID: {idVenta}");
                    TempData["Success"] = "¡Pedido procesado exitosamente!";
                    return RedirectToAction("Confirmacion", new { id = idVenta });
                }
                else
                {
                    Console.WriteLine("DEBUG - Error: ProcesarPedidoAsync devolvió 0");
                    ModelState.AddModelError("", "Error al procesar el pedido. Intente nuevamente.");
                    return await RecargarCheckoutConError(model, idCliente);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG - Error en ProcesarPedido: {ex.Message}");
                Console.WriteLine($"DEBUG - InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"DEBUG - StackTrace: {ex.StackTrace}");

                ModelState.AddModelError("", $"Error al procesar el pedido: {ex.Message}");
                return await RecargarCheckoutConError(model, idCliente);
            }
        }

        // GET: Confirmacion de pedido
        public async Task<IActionResult> Confirmacion(int id)
        {
            try
            {
                var idCliente = GetClienteId();
                var confirmacion = await _checkoutService.ObtenerConfirmacionPedidoAsync(id);

                if (confirmacion == null)
                {
                    TempData["Error"] = "Pedido no encontrado";
                    return RedirectToAction("Index", "Catalogo");
                }

                return View(confirmacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener la confirmación: " + ex.Message;
                return RedirectToAction("Index", "Catalogo");
            }
        }

        // GET: Resumen del pedido (AJAX)
        public async Task<IActionResult> ObtenerResumen()
        {
            try
            {
                var idCliente = GetClienteId();
                var checkout = await _checkoutService.ObtenerCheckoutAsync(idCliente);

                var resumen = new ResumenPedidoViewModel
                {
                    Items = checkout.ItemsCarrito,
                    Subtotal = checkout.Subtotal,
                    CostoEnvio = checkout.CostoEnvio,
                    DescuentoAplicado = checkout.DescuentoAplicado,
                    Total = checkout.Total,
                    TotalItems = checkout.TotalItems
                };

                return PartialView("_ResumenPedidoPartial", resumen);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #region Métodos Helper

        /// <summary>
        /// Recarga la vista de checkout con los datos actualizados cuando hay errores
        /// </summary>
        private async Task<IActionResult> RecargarCheckoutConError(CheckoutViewModel model, int idCliente)
        {
            try
            {
                Console.WriteLine($"DEBUG - RecargarCheckout IdCliente: {idCliente}");

                // Recargar datos frescos desde la base de datos
                var checkoutFresco = await _checkoutService.ObtenerCheckoutAsync(idCliente);

                Console.WriteLine($"DEBUG - Items carrito: {checkoutFresco.ItemsCarrito.Count}");
                Console.WriteLine($"DEBUG - Direcciones: {checkoutFresco.DireccionesDisponibles.Count}");
                Console.WriteLine($"DEBUG - Métodos pago: {checkoutFresco.MetodosPagoDisponibles.Count}");

                // Preservar selecciones del usuario
                checkoutFresco.MetodoPagoSeleccionado = model.MetodoPagoSeleccionado;
                checkoutFresco.DireccionEnvioSeleccionada = model.DireccionEnvioSeleccionada;
                checkoutFresco.UsarNuevaDireccion = model.UsarNuevaDireccion;
                checkoutFresco.AceptaTerminos = model.AceptaTerminos;
                checkoutFresco.NotasEspeciales = model.NotasEspeciales;
                checkoutFresco.CodigoCupon = model.CodigoCupon;
                checkoutFresco.CostoEnvio = model.CostoEnvio;
                checkoutFresco.DescuentoAplicado = model.DescuentoAplicado;

                // Preservar datos de nueva dirección si los había
                if (model.UsarNuevaDireccion && model.NuevaDireccion != null)
                {
                    checkoutFresco.NuevaDireccion = model.NuevaDireccion;
                }

                // Calcular total
                checkoutFresco.Total = checkoutFresco.Subtotal + checkoutFresco.CostoEnvio - checkoutFresco.DescuentoAplicado;

                return View("Index", checkoutFresco);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG - Error en RecargarCheckout: {ex.Message}");
                TempData["Error"] = "Error al cargar el checkout: " + ex.Message;
                return RedirectToAction("Index", "Carrito");
            }
        }

        /// <summary>
        /// Renderiza una vista parcial a string para retornar por AJAX
        /// </summary>
        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                var viewEngine = HttpContext.RequestServices.GetService<ICompositeViewEngine>();
                var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

                if (!viewResult.Success)
                {
                    return string.Empty;
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.ToString();
            }
        }

        #endregion
    }
}