
using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly DbecommerceContext _context;

        public VentaService(DbecommerceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VentaDetalleViewModel>> ObtenerTodasLasVentasAsync()
        {
            var ventas = await _context.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(dv => dv.IdProductoNavigation)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            var ventasDetalle = new List<VentaDetalleViewModel>();

            foreach (var venta in ventas)
            {
                foreach (var detalle in venta.DetalleVenta)
                {
                    ventasDetalle.Add(new VentaDetalleViewModel
                    {
                        IdVenta = venta.IdVenta,
                        FechaVenta = venta.FechaVenta ?? DateTime.Now,
                        IdCliente = venta.IdCliente ?? 0,
                        ClienteNombre = $"{venta.IdClienteNavigation?.Nombres} {venta.IdClienteNavigation?.Apellidos}",
                        IdProducto = detalle.IdProducto ?? 0,
                        ProductoNombre = detalle.IdProductoNavigation?.Nombre ?? "Sin nombre",
                        Precio = detalle.Total ?? 0 / (detalle.Cantidad ?? 1),
                        TotalProductos = detalle.Cantidad ?? 0,
                        ImporteTotal = detalle.Total ?? 0,
                        IdTransaccion = venta.IdTransaccion ?? "N/A"
                    });
                }
            }

            return ventasDetalle;
        }

        public async Task<VentaDetalleViewModel> ObtenerVentaPorIdAsync(int idVenta)
        {
            var venta = await _context.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(dv => dv.IdProductoNavigation)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta == null) return null;

            var detalle = venta.DetalleVenta.FirstOrDefault();

            return new VentaDetalleViewModel
            {
                IdVenta = venta.IdVenta,
                FechaVenta = venta.FechaVenta ?? DateTime.Now,
                IdCliente = venta.IdCliente ?? 0,
                ClienteNombre = $"{venta.IdClienteNavigation?.Nombres} {venta.IdClienteNavigation?.Apellidos}",
                IdProducto = detalle?.IdProducto ?? 0,
                ProductoNombre = detalle?.IdProductoNavigation?.Nombre ?? "Sin nombre",
                Precio = detalle?.Total ?? 0 / (detalle?.Cantidad ?? 1),
                TotalProductos = venta.TotalProductos ?? 0,
                ImporteTotal = venta.ImporteTotal ?? 0,
                IdTransaccion = venta.IdTransaccion ?? "N/A"
            };
        }

        public async Task<Dashboard> ObtenerEstadisticasDashboardAsync()
        {
            var totalVentas = await _context.Ventas.SumAsync(v => v.ImporteTotal ?? 0);
            var totalClientes = await _context.Clientes.CountAsync();
            var totalProductosVendidos = await _context.DetalleVentas.SumAsync(dv => dv.Cantidad ?? 0);

            return new Dashboard
            {
                TotalVenta = totalVentas,
                TotalCliente = totalClientes,
                TotalProducto = totalProductosVendidos
            };
        }
    }
}