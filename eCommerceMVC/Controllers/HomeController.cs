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

        // 👉 Método para generar ventas de ejemplo con varios productos por venta
        private List<VentaDetalleViewModel> GenerarVentasEjemplo(List<Usuario> usuarios, List<Producto> productos)
        {
            var ventas = new List<VentaDetalleViewModel>();
            var rnd = new Random();

            for (int i = 0; i < usuarios.Count; i++)
            {
                var usuario = usuarios[i];
                int numProductos = rnd.Next(2, 6); // 2 a 5 productos por venta

                // Seleccionamos productos aleatorios sin repetir en la misma venta
                var productosVenta = productos.OrderBy(x => rnd.Next()).Take(numProductos).ToList();

                foreach (var producto in productosVenta)
                {
                    int cantidad = rnd.Next(1, 4); // 1 a 3 unidades
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

        // Exportar UNA sola venta (agrupando sus productos)
        public async Task<IActionResult> ExportarVentaPdf(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            // Agrupamos productos por venta
            var ventaCompleta = ventas
                .Where(v => v.IdVenta == id)
                .ToList();

            if (!ventaCompleta.Any())
                return NotFound();

            var exporter = new VentaPdfExporter();
            var pdfBytes = exporter.GenerarPdfVentaCompleta(ventaCompleta);

            return File(pdfBytes, "application/pdf", $"Venta_{id}.pdf");
        }

        // Exportar TODAS las ventas en un solo PDF
        public async Task<IActionResult> ExportarTodasLasVentas()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var usuarios = (await _usuarioService.GetAllAsync()).ToList();
            var productos = (await _productoService.GetAllAsync()).ToList();

            var ventas = GenerarVentasEjemplo(usuarios, productos);

            // Agrupamos por IdVenta para que cada venta tenga sus productos
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
