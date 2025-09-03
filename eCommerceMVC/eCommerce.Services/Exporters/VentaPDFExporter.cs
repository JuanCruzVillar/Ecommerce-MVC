using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Globalization;
using eCommerce.Entities.ViewModels;

namespace eCommerceMVC.Services.Exporters
{
    public class VentaPdfExporter
    {
        public byte[] GenerarPdf(VentaDetalleViewModel venta)
        {
            var stream = new MemoryStream();
            var cultura = new CultureInfo("es-AR"); 

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"Detalle de Venta #{venta.IdVenta}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Column(col =>
                        {
                            col.Item().Text($"Fecha: {venta.FechaVenta:dd/MM/yyyy}");
                            col.Item().Text($"Cliente: {venta.ClienteNombre}");
                            col.Item().Text($"Producto: {venta.ProductoNombre}");
                            
                            col.Item().Text($"Precio Unitario: {venta.Precio.ToString("C", cultura)}");
                            col.Item().Text($"Cantidad: {venta.TotalProductos}");
                            col.Item().Text($"Importe Total: {venta.ImporteTotal.ToString("C", cultura)}");
                            col.Item().Text($"ID Transacción: {venta.IdTransaccion}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Hardware Store");
                });
            })
            .GeneratePdf(stream);

            return stream.ToArray();
        }
        public byte[] GenerarPdfTodas(List<VentaDetalleViewModel> ventas)
        {
            var stream = new MemoryStream();
            var cultura = new CultureInfo("es-AR");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Reporte de Ventas")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Column(col =>
                        {
                            foreach (var venta in ventas)
                            {
                                col.Item().BorderBottom(1).Padding(5).Column(item =>
                                {
                                    item.Item().Text($"Venta #{venta.IdVenta} - {venta.FechaVenta:dd/MM/yyyy}");
                                    item.Item().Text($"Cliente: {venta.ClienteNombre}");
                                    item.Item().Text($"Producto: {venta.ProductoNombre}");
                                    item.Item().Text($"Precio Unitario: {venta.Precio.ToString("C", cultura)}");
                                    item.Item().Text($"Cantidad: {venta.TotalProductos}");
                                    item.Item().Text($"Importe Total: {venta.ImporteTotal.ToString("C", cultura)}");
                                    item.Item().Text($"ID Transacción: {venta.IdTransaccion}");
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Reporte generado desde Mi Portfolio de eCommerce MVC - Demo");
                });
            })
            .GeneratePdf(stream);

            return stream.ToArray();
        }

    }
}
