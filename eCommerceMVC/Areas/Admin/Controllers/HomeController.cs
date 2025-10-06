using eCommerce.Areas.Admin.Controllers;
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using eCommerceMVC.Services.Exporters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;

        public HomeController(IUsuarioService usuarioService, IProductoService productoService)
        {
            _usuarioService = usuarioService;
            _productoService = productoService;
        }

        private List<VentaDetalleViewModel> GenerarVentasEjemplo(List<Usuario> usuarios, List<Producto> productos)
        {
            var ventas = new List<VentaDetalleViewModel>();
            var rnd = new Random();

            for (int i = 0; i < usuarios.Count; i++)
            {
                var usuario = usuarios[i];
                int numProductos = rnd.Next(2, 6);

                var productosVenta = productos.OrderBy(x => rnd.Next()).Take(numProductos).ToList();

                foreach (var producto in productosVenta)
                {
                    int cantidad = rnd.Next(1, 4);
                    decimal importe = (producto.Precio ?? 0m) * cantidad;

                    ventas.Add(new VentaDetalleViewModel
                    {
                        IdVenta = i + 1,
                        FechaVenta = DateTime.Now.AddDays(-i),
                        ClienteNombre = usuario.Nombres + " " + usuario.Apellidos,
                        ProductoNombre = producto.Nombre,
                        Precio = producto.Precio ?? 0m,
                        TotalProductos = cantidad,
                        ImporteTotal = importe,
                        IdTransaccion = "TXN00" + (i + 1)
                    });
                }
            }

            return ventas;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            var dashboard = new Dashboard
            {
                TotalVenta = ventas.Sum(v => v.ImporteTotal),
                TotalCliente = usuarios.Count,
                TotalProducto = ventas.Sum(v => v.TotalProductos)
            };

            var model = new VentasViewModel
            {
                Resumen = dashboard,
                Detalles = ventas
            };

            return View(model);
        }

        public async Task<IActionResult> ExportarVentaPdf(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            var ventaCompleta = ventas
                .Where(v => v.IdVenta == id)
                .ToList();

            if (!ventaCompleta.Any())
                return NotFound();

            var exporter = new VentaPdfExporter();
            var pdfBytes = exporter.GenerarPdfVentaCompleta(ventaCompleta);

            return File(pdfBytes, "application/pdf", $"Venta_{id}.pdf");
        }

        public async Task<IActionResult> ExportarTodasLasVentas()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            var ventasAgrupadas = ventas
                .GroupBy(v => v.IdVenta)
                .Select(g => g.ToList())
                .ToList();

            var exporter = new VentaPdfExporter();
            var pdfBytes = exporter.GenerarPdfTodasVentas(ventasAgrupadas);

            return File(pdfBytes, "application/pdf", "Reporte_Ventas.pdf");
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