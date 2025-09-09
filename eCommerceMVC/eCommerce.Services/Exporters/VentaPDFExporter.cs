using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Globalization;
using eCommerce.Entities.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace eCommerceMVC.Services.Exporters
{
    public class VentaPdfExporter
    {
        private readonly CultureInfo cultura = new CultureInfo("es-AR");

        // Exportar UNA venta completa con múltiples productos
        public byte[] GenerarPdfVentaCompleta(List<VentaDetalleViewModel> productosVenta)
        {
            if (productosVenta == null || !productosVenta.Any())
                return null;

            var stream = new MemoryStream();
            var venta = productosVenta.First(); // Tomamos los datos generales de la venta (cliente, fecha, ID)

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // HEADER
                    page.Header()
                        .Column(col =>
                        {
                            col.Item().Text("Hardware Store").SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);
                            col.Item().Text("Calle Falsa 123, Ciudad, Argentina");
                            col.Item().Text("Tel: +54 9 11 1234-5678 | Email: contacto@hardwarestore.com");
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                    // INFORMACIÓN DE LA VENTA
                    page.Content()
                        .Column(col =>
                        {
                            col.Item().Text($"Factura N°: {venta.IdVenta}").SemiBold();
                            col.Item().Text($"Fecha: {venta.FechaVenta:dd/MM/yyyy}");
                            col.Item().Text($"Cliente: {venta.ClienteNombre}");
                            col.Item().Text($"ID Transacción: {venta.IdTransaccion}");
                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            // TABLA DE PRODUCTOS
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4); // Producto
                                    columns.ConstantColumn(60); // Cantidad
                                    columns.ConstantColumn(80); // Precio Unitario
                                    columns.ConstantColumn(80); // Importe Total
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Text("Producto").SemiBold();
                                    header.Cell().Text("Cant.").SemiBold();
                                    header.Cell().Text("Precio Unit.").SemiBold();
                                    header.Cell().Text("Total").SemiBold();
                                });

                                // Filas
                                foreach (var item in productosVenta)
                                {
                                    table.Cell().Text(item.ProductoNombre);
                                    table.Cell().AlignRight().Text(item.TotalProductos.ToString());
                                    table.Cell().AlignRight().Text(item.Precio.ToString("C", cultura));
                                    table.Cell().AlignRight().Text(item.ImporteTotal.ToString("C", cultura));
                                }

                                // TOTAL
                                table.Footer(footer =>
                                {
                                    footer.Cell().ColumnSpan(3).AlignRight().Text("Total").SemiBold();
                                    footer.Cell().AlignRight().Text(productosVenta.Sum(p => p.ImporteTotal).ToString("C", cultura)).SemiBold();
                                });
                            });
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text("Gracias por su compra. Hardware Store - Calle Falsa 123, Ciudad, Tel: +54 9 11 1234-5678");
                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }

        // Exportar TODAS las ventas, cada una con múltiples productos
        public byte[] GenerarPdfTodasVentas(List<List<VentaDetalleViewModel>> ventasAgrupadas)
        {
            var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // HEADER
                    page.Header()
                        .Column(col =>
                        {
                            col.Item().Text("Hardware Store").SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);
                            col.Item().Text("Calle Falsa 123, Ciudad, Argentina");
                            col.Item().Text("Tel: +54 9 11 1234-5678 | Email: contacto@hardwarestore.com");
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                    // CONTENIDO
                    page.Content()
                        .Column(col =>
                        {
                            foreach (var venta in ventasAgrupadas)
                            {
                                var datosVenta = venta.First();

                                col.Item().PaddingVertical(5).BorderBottom(1).Column(item =>
                                {
                                    item.Item().Text($"Factura N°: {datosVenta.IdVenta}").SemiBold();
                                    item.Item().Text($"Fecha: {datosVenta.FechaVenta:dd/MM/yyyy}");
                                    item.Item().Text($"Cliente: {datosVenta.ClienteNombre}");
                                    item.Item().Text($"ID Transacción: {datosVenta.IdTransaccion}");
                                    item.Item().PaddingVertical(2);

                                    // Tabla de productos
                                    item.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(4);
                                            columns.ConstantColumn(60);
                                            columns.ConstantColumn(80);
                                            columns.ConstantColumn(80);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Text("Producto").SemiBold();
                                            header.Cell().Text("Cant.").SemiBold();
                                            header.Cell().Text("Precio Unit.").SemiBold();
                                            header.Cell().Text("Total").SemiBold();
                                        });

                                        foreach (var prod in venta)
                                        {
                                            table.Cell().Text(prod.ProductoNombre);
                                            table.Cell().AlignRight().Text(prod.TotalProductos.ToString());
                                            table.Cell().AlignRight().Text(prod.Precio.ToString("C", cultura));
                                            table.Cell().AlignRight().Text(prod.ImporteTotal.ToString("C", cultura));
                                        }

                                        table.Footer(footer =>
                                        {
                                            footer.Cell().ColumnSpan(3).AlignRight().Text("Total").SemiBold();
                                            footer.Cell().AlignRight().Text(venta.Sum(p => p.ImporteTotal).ToString("C", cultura)).SemiBold();
                                        });
                                    });
                                });
                            }
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text("Reporte generado desde Hardware Store - Demo");
                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }
    }
}
