using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceMVC.Services.Exporters;

namespace eCommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;

        public HomeController(IUsuarioService usuarioService, IProductoService productoService)
        {
            _usuarioService = usuarioService;
            _productoService = productoService;
        }

        // 👉 Método reutilizable para generar ventas de ejemplo
        private List<VentaDetalleViewModel> GenerarVentasEjemplo(List<Usuario> usuarios, List<Producto> productos)
        {
            var ventas = new List<VentaDetalleViewModel>();

            for (int i = 0; i < usuarios.Count; i++)
            {
                var producto = productos[i % productos.Count];
                var usuario = usuarios[i];

                ventas.Add(new VentaDetalleViewModel
                {
                    IdVenta = i + 1,
                    FechaVenta = DateTime.Now.AddDays(-i),
                    ClienteNombre = usuario.Nombres + " " + usuario.Apellidos,
                    ProductoNombre = producto.Nombre,
                    Precio = producto.Precio ?? 0m,
                    TotalProductos = (i % 3) + 1,
                    ImporteTotal = (producto.Precio ?? 0m) * ((i % 3) + 1),
                    IdTransaccion = "TXN00" + (i + 1)
                });
            }

            return ventas;
        }

        // Dashboard principal
        public async Task<IActionResult> Index()
        {
            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            var dashboard = new Dashbord
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

        // Exportar UNA sola venta
        public async Task<IActionResult> ExportarVentaPdf(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            var venta = ventas.FirstOrDefault(v => v.IdVenta == id);
            if (venta == null)
                return NotFound();

            var exporter = new VentaPdfExporter();
            var pdfBytes = exporter.GenerarPdf(venta);

            return File(pdfBytes, "application/pdf", $"Venta_{venta.IdVenta}.pdf");
        }

        // Exportar TODAS las ventas en un solo PDF
        public async Task<IActionResult> ExportarTodasLasVentas()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            var exporter = new VentaPdfExporter();
            var pdfBytes = exporter.GenerarPdfTodas(ventas);

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
