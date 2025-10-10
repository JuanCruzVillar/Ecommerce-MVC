using eCommerce.Areas.Admin.Controllers;
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using eCommerceMVC.Services.Exporters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Data;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        private readonly DbecommerceContext _context;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;

        public HomeController(
            DbecommerceContext context,
            IUsuarioService usuarioService,
            IProductoService productoService)
        {
            _context = context;
            _usuarioService = usuarioService;
            _productoService = productoService;
        }

        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int? idEstado)
        {
            try
            {
                // Definir rango de fechas (por defecto últimos 30 días)
                var inicio = fechaInicio ?? DateTime.Now.AddDays(-30);
                var fin = fechaFin ?? DateTime.Now.AddDays(1);

                // Obtener ventas reales de la BD
                var ventasQuery = _context.Ventas
                    .Include(v => v.IdClienteNavigation)
                    .Include(v => v.IdMetodoPagoNavigation)
                    .Include(v => v.IdEstadoPedidoNavigation)
                    .Include(v => v.IdDireccionEnvioNavigation)
                    .Include(v => v.DetalleVenta)
                        .ThenInclude(dv => dv.IdProductoNavigation)
                    .AsQueryable();

                // Filtrar por rango de fechas
                ventasQuery = ventasQuery.Where(v => v.FechaVenta >= inicio && v.FechaVenta <= fin);

                // Filtrar por estado si se especifica
                if (idEstado.HasValue && idEstado > 0)
                {
                    ventasQuery = ventasQuery.Where(v => v.IdEstadoPedido == idEstado);
                }

                var ventasReales = await ventasQuery
                    .OrderByDescending(v => v.FechaVenta)
                    .ToListAsync();

                // Construir lista de detalles (una fila por producto en la venta)
                var detalles = new List<VentaDetalleViewModel>();
                foreach (var venta in ventasReales)
                {
                    if (venta.DetalleVenta.Any())
                    {
                        foreach (var detalle in venta.DetalleVenta)
                        {
                            detalles.Add(new VentaDetalleViewModel
                            {
                                IdVenta = venta.IdVenta,
                                FechaVenta = venta.FechaVenta ?? DateTime.Now,
                                ClienteNombre = $"{venta.IdClienteNavigation?.Nombres} {venta.IdClienteNavigation?.Apellidos}",
                                ClienteCorreo = venta.IdClienteNavigation?.Correo,
                                ProductoNombre = detalle.IdProductoNavigation?.Nombre ?? "N/A",
                                Precio = detalle.IdProductoNavigation?.Precio ?? 0,
                                TotalProductos = detalle.Cantidad ?? 0,
                                ImporteTotal = detalle.Total ?? 0,
                                IdTransaccion = venta.IdTransaccion ?? "N/A",
                                EstadoPedido = venta.IdEstadoPedidoNavigation?.Nombre ?? "Pendiente",
                                MetodoPago = venta.IdMetodoPagoNavigation?.Nombre ?? "N/A",
                                DescuentoAplicado = venta.DescuentoAplicado ?? 0,
                                CostoEnvio = venta.CostoEnvio ?? 0
                            });
                        }
                    }
                    else
                    {
                        // Si no hay detalles, agregar una fila con los totales de la venta
                        detalles.Add(new VentaDetalleViewModel
                        {
                            IdVenta = venta.IdVenta,
                            FechaVenta = venta.FechaVenta ?? DateTime.Now,
                            ClienteNombre = $"{venta.IdClienteNavigation?.Nombres} {venta.IdClienteNavigation?.Apellidos}",
                            ClienteCorreo = venta.IdClienteNavigation?.Correo,
                            ProductoNombre = "Venta múltiple",
                            TotalProductos = venta.TotalProductos ?? 0,
                            ImporteTotal = venta.ImporteTotal ?? 0,
                            IdTransaccion = venta.IdTransaccion ?? "N/A",
                            EstadoPedido = venta.IdEstadoPedidoNavigation?.Nombre ?? "Pendiente",
                            MetodoPago = venta.IdMetodoPagoNavigation?.Nombre ?? "N/A",
                            DescuentoAplicado = venta.DescuentoAplicado ?? 0,
                            CostoEnvio = venta.CostoEnvio ?? 0
                        });
                    }
                }

                // Calcular métricas
                var totalVentas = ventasReales.Sum(v => v.ImporteTotal ?? 0);
                var ventasHoy = ventasReales.Count(v => v.FechaVenta.HasValue && v.FechaVenta.Value.Date == DateTime.Today);
                var clientesUnicos = ventasReales.Select(v => v.IdCliente).Distinct().Count();
                var productosVendidos = ventasReales.Sum(v => v.TotalProductos ?? 0);

                var dashboard = new Dashboard
                {
                    TotalVenta = totalVentas,
                    TotalCliente = clientesUnicos,
                    TotalProducto = productosVendidos,
                    VentasHoy = ventasHoy,
                    TicketPromedio = ventasReales.Count > 0 ? totalVentas / ventasReales.Count : 0
                };

                var model = new VentasViewModel
                {
                    Resumen = dashboard,
                    Detalles = detalles,
                    FechaInicio = inicio,
                    FechaFin = fin,
                    EstadoSeleccionado = idEstado ?? 0
                };

                // Estados para el filtro
                ViewBag.Estados = await _context.EstadosPedido.ToListAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Dashboard: {ex.Message}");
                TempData["Error"] = "Error al cargar el dashboard";
                return View(new VentasViewModel { Detalles = new List<VentaDetalleViewModel>() });
            }
        }

        public async Task<IActionResult> ExportarVentaPdf(int id)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                var venta = await _context.Ventas
                    .Include(v => v.IdClienteNavigation)
                    .Include(v => v.IdMetodoPagoNavigation)
                    .Include(v => v.IdEstadoPedidoNavigation)
                    .Include(v => v.DetalleVenta)
                        .ThenInclude(dv => dv.IdProductoNavigation)
                    .FirstOrDefaultAsync(v => v.IdVenta == id);

                if (venta == null)
                    return NotFound();

                // Convertir a VentaDetalleViewModel
                var ventaDetalles = venta.DetalleVenta
                    .Select(d => new VentaDetalleViewModel
                    {
                        IdVenta = venta.IdVenta,
                        FechaVenta = venta.FechaVenta ?? DateTime.Now,
                        ClienteNombre = $"{venta.IdClienteNavigation?.Nombres} {venta.IdClienteNavigation?.Apellidos}",
                        ProductoNombre = d.IdProductoNavigation?.Nombre ?? "N/A",
                        Precio = d.IdProductoNavigation?.Precio ?? 0,
                        TotalProductos = d.Cantidad ?? 0,
                        ImporteTotal = d.Total ?? 0,
                        IdTransaccion = venta.IdTransaccion ?? "N/A"
                    })
                    .ToList();

                var exporter = new VentaPdfExporter();
                var pdfBytes = exporter.GenerarPdfVentaCompleta(ventaDetalles);

                return File(pdfBytes, "application/pdf", $"Venta_{id}.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exportando PDF: {ex.Message}");
                TempData["Error"] = "Error al exportar PDF";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> ExportarTodasLasVentas(DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                var inicio = fechaInicio ?? DateTime.Now.AddDays(-30);
                var fin = fechaFin ?? DateTime.Now.AddDays(1);

                var ventas = await _context.Ventas
                    .Include(v => v.IdClienteNavigation)
                    .Include(v => v.IdMetodoPagoNavigation)
                    .Include(v => v.DetalleVenta)
                        .ThenInclude(dv => dv.IdProductoNavigation)
                    .Where(v => v.FechaVenta >= inicio && v.FechaVenta <= fin)
                    .OrderByDescending(v => v.FechaVenta)
                    .ToListAsync();

                // Agrupar por venta
                var ventasAgrupadas = new List<List<VentaDetalleViewModel>>();
                foreach (var venta in ventas)
                {
                    var detalles = venta.DetalleVenta
                        .Select(d => new VentaDetalleViewModel
                        {
                            IdVenta = venta.IdVenta,
                            FechaVenta = venta.FechaVenta ?? DateTime.Now,
                            ClienteNombre = $"{venta.IdClienteNavigation?.Nombres} {venta.IdClienteNavigation?.Apellidos}",
                            ProductoNombre = d.IdProductoNavigation?.Nombre ?? "N/A",
                            Precio = d.IdProductoNavigation?.Precio ?? 0,
                            TotalProductos = d.Cantidad ?? 0,
                            ImporteTotal = d.Total ?? 0,
                            IdTransaccion = venta.IdTransaccion ?? "N/A"
                        })
                        .ToList();

                    ventasAgrupadas.Add(detalles);
                }

                var exporter = new VentaPdfExporter();
                var pdfBytes = exporter.GenerarPdfTodasVentas(ventasAgrupadas);

                return File(pdfBytes, "application/pdf", $"Reporte_Ventas_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exportando reporte: {ex.Message}");
                TempData["Error"] = "Error al exportar reporte";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Usuarios()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}