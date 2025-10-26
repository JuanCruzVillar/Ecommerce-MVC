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
using Microsoft.Extensions.Logging;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class CheckoutController : BaseNegocioController
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICarritoService _carritoService;
        private readonly DbecommerceContext _context;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            ICheckoutService checkoutService,
            ICarritoService carritoService,
            DbecommerceContext context,
            ILogger<CheckoutController> logger)
        {
            _checkoutService = checkoutService;
            _carritoService = carritoService;
            _context = context;
            _logger = logger;
        }

        // GET: /Cliente/Checkout
        public async Task<IActionResult> Index()
        {
            try
            {
                var idCliente = GetClienteId();

                if (idCliente == 0)
                {
                    TempData["Error"] = "Debe iniciar sesión para continuar con la compra";
                    return RedirectToAction("Login", "Auth");
                }

                var itemsCarrito = await _carritoService.ObtenerItemsCarritoAsync(idCliente);

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

                if (!checkoutViewModel.DireccionesDisponibles.Any())
                {
                    checkoutViewModel.UsarNuevaDireccion = true;
                }

                return View(checkoutViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar checkout para cliente");
                TempData["Error"] = "Error al cargar el checkout. Por favor, intente nuevamente.";
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
                _logger.LogError(ex, "Error al validar cupón {Codigo}", cuponModel.Codigo);
                return Json(new
                {
                    exito = false,
                    mensaje = "Error al validar el cupón. Por favor, intente nuevamente."
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
                _logger.LogError(ex, "Error al agregar dirección para cliente");
                return Json(new
                {
                    exito = false,
                    mensaje = "Error al procesar la solicitud. Por favor, intente nuevamente."
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
                _logger.LogError(ex, "Error al calcular envío para dirección {DireccionId}", idDireccionEnvio);
                return Json(new
                {
                    exito = false,
                    mensaje = "Error al calcular el costo de envío"
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
                // Limpiar ModelState de propiedades calculadas
                ModelState.Remove("Cliente");
                ModelState.Remove("ItemsCarrito");
                ModelState.Remove("DireccionesDisponibles");
                ModelState.Remove("MetodosPagoDisponibles");
                ModelState.Remove("CodigoCupon");
                ModelState.Remove("MensajeCupon");
                ModelState.Remove("NotasEspeciales");
                ModelState.Remove("CantidadCuotas");
                ModelState.Remove("Subtotal");
                ModelState.Remove("CostoEnvio");
                ModelState.Remove("Total");

                //  RECALCULAR TODO EN SERVIDOR
                var checkoutActualizado = await _checkoutService.ObtenerCheckoutAsync(idCliente);

                //  Validar que el carrito no esté vacío
                if (!checkoutActualizado.ItemsCarrito.Any())
                {
                    ModelState.AddModelError("", "El carrito está vacío");
                    return await RecargarCheckoutConError(model, idCliente);
                }

                //  Validar método de pago
                if (model.MetodoPagoSeleccionado == 0)
                {
                    ModelState.AddModelError("MetodoPagoSeleccionado", "Debe seleccionar un método de pago");
                    return await RecargarCheckoutConError(model, idCliente);
                }

                //  Validar y crear dirección si es nueva
                if (model.UsarNuevaDireccion)
                {
                    if (model.NuevaDireccion == null)
                    {
                        ModelState.AddModelError("", "Los datos de la dirección son requeridos");
                        return await RecargarCheckoutConError(model, idCliente);
                    }

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
                        foreach (var error in erroresDireccion)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return await RecargarCheckoutConError(model, idCliente);
                    }

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
                }
                else
                {
                    //  Validar dirección existente
                    if (!model.DireccionEnvioSeleccionada.HasValue || model.DireccionEnvioSeleccionada == 0)
                    {
                        ModelState.AddModelError("DireccionEnvioSeleccionada", "Debe seleccionar una dirección de envío");
                        return await RecargarCheckoutConError(model, idCliente);
                    }

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

                //  Validar términos
                if (!model.AceptaTerminos)
                {
                    ModelState.AddModelError("AceptaTerminos", "Debe aceptar los términos y condiciones");
                    return await RecargarCheckoutConError(model, idCliente);
                }

                //  Validar stock disponible
                var stockDisponible = await _checkoutService.ValidarStockProductosAsync(idCliente);
                if (!stockDisponible)
                {
                    ModelState.AddModelError("", "Algunos productos ya no tienen stock disponible");
                    return await RecargarCheckoutConError(model, idCliente);
                }

               
                model.ItemsCarrito = checkoutActualizado.ItemsCarrito;
                model.Subtotal = checkoutActualizado.Subtotal;
                model.TotalItems = checkoutActualizado.TotalItems;

                //  Recalcular costo de envío
                if (model.DireccionEnvioSeleccionada.HasValue)
                {
                    model.CostoEnvio = await _checkoutService.CalcularCostoEnvioAsync(model.DireccionEnvioSeleccionada.Value);
                }

                //  Validar y aplicar cupón 
                model.DescuentoAplicado = 0;
                if (!string.IsNullOrEmpty(model.CodigoCupon))
                {
                    var resultadoCupon = await _checkoutService.ValidarCuponAsync(model.CodigoCupon, model.Subtotal);
                    if (resultadoCupon.EsValido)
                    {
                        model.DescuentoAplicado = resultadoCupon.DescuentoAplicado;
                        model.CuponAplicado = true;
                    }
                    else
                    {
                        _logger.LogWarning("Cupón inválido '{Cupon}' para cliente {ClienteId}", model.CodigoCupon, idCliente);
                        model.CodigoCupon = null; // Ignorar cupón inválido
                    }
                }

                //  CALCULAR TOTAL FINAL (NUNCA CONFIAR EN EL CLIENTE)
                model.Total = model.Subtotal + model.CostoEnvio - model.DescuentoAplicado;

                //  Validación adicional: total debe ser positivo
                if (model.Total < 0)
                {
                    _logger.LogError("Total negativo detectado para cliente {ClienteId}. Subtotal: {Subtotal}, Envío: {Envio}, Descuento: {Descuento}",
                        idCliente, model.Subtotal, model.CostoEnvio, model.DescuentoAplicado);
                    ModelState.AddModelError("", "Error en el cálculo del total. Por favor, intente nuevamente.");
                    return await RecargarCheckoutConError(model, idCliente);
                }

                //  Procesar pedido con datos validados
                var idVenta = await _checkoutService.ProcesarPedidoAsync(model, idCliente);

                if (idVenta > 0)
                {
                    _logger.LogInformation("Pedido {VentaId} procesado exitosamente. Cliente: {ClienteId}, Total: {Total:C}",
                        idVenta, idCliente, model.Total);
                    TempData["Success"] = "¡Pedido procesado exitosamente!";
                    return RedirectToAction("Confirmacion", new { id = idVenta });
                }
                else
                {
                    _logger.LogWarning("Fallo al procesar pedido para cliente {ClienteId}", idCliente);
                    ModelState.AddModelError("", "Error al procesar el pedido. Intente nuevamente.");
                    return await RecargarCheckoutConError(model, idCliente);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al procesar pedido para cliente {ClienteId}", idCliente);
                ModelState.AddModelError("", "Error al procesar el pedido. Por favor, intente nuevamente.");
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
                _logger.LogError(ex, "Error al obtener confirmación del pedido {PedidoId}", id);
                TempData["Error"] = "Error al obtener la confirmación del pedido";
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
                _logger.LogError(ex, "Error al obtener resumen del pedido");
                return Json(new { error = "Error al cargar el resumen" });
            }
        }

        #region Métodos Helper

        private async Task<IActionResult> RecargarCheckoutConError(CheckoutViewModel model, int idCliente)
        {
            try
            {
                var checkoutFresco = await _checkoutService.ObtenerCheckoutAsync(idCliente);

                checkoutFresco.MetodoPagoSeleccionado = model.MetodoPagoSeleccionado;
                checkoutFresco.DireccionEnvioSeleccionada = model.DireccionEnvioSeleccionada;
                checkoutFresco.UsarNuevaDireccion = model.UsarNuevaDireccion;
                checkoutFresco.AceptaTerminos = model.AceptaTerminos;
                checkoutFresco.NotasEspeciales = model.NotasEspeciales;
                checkoutFresco.CodigoCupon = model.CodigoCupon;
                checkoutFresco.CostoEnvio = model.CostoEnvio;
                checkoutFresco.DescuentoAplicado = model.DescuentoAplicado;
                checkoutFresco.CantidadCuotas = model.CantidadCuotas;

                if (model.UsarNuevaDireccion && model.NuevaDireccion != null)
                {
                    checkoutFresco.NuevaDireccion = model.NuevaDireccion;
                }

                checkoutFresco.Total = checkoutFresco.Subtotal + checkoutFresco.CostoEnvio - checkoutFresco.DescuentoAplicado;

                return View("Index", checkoutFresco);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recargar checkout para cliente {ClienteId}", idCliente);
                TempData["Error"] = "Error al cargar el checkout. Por favor, intente nuevamente.";
                return RedirectToAction("Index", "Carrito");
            }
        }

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