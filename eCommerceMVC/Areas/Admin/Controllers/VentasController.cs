using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace eCommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VentasController : Controller
    {
        public IActionResult Index()
        {
            // Dashboard ficticio
            var dashboard = new Dashboard
            {
                TotalVenta = 120000,
                TotalCliente = 15,
                TotalProducto = 25
            };

            // Lista de ventas ficticias
            var ventas = new List<VentaDetalleViewModel>
            {
                new VentaDetalleViewModel { IdVenta = 1, FechaVenta = DateTime.Now.AddDays(-1), IdCliente = 101, IdProducto = 5, Precio = 500, TotalProductos = 2, ImporteTotal = 1000, IdTransaccion = "TXN001" },
                new VentaDetalleViewModel { IdVenta = 2, FechaVenta = DateTime.Now.AddDays(-2), IdCliente = 102, IdProducto = 7, Precio = 300, TotalProductos = 3, ImporteTotal = 900, IdTransaccion = "TXN002" },
                new VentaDetalleViewModel { IdVenta = 3, FechaVenta = DateTime.Now.AddDays(-3), IdCliente = 103, IdProducto = 2, Precio = 150, TotalProductos = 5, ImporteTotal = 750, IdTransaccion = "TXN003" }
            };

            // Crear el ViewModel completo
            var model = new VentasViewModel
            {
                Resumen = dashboard,
                Detalles = ventas
            };

            // Pasar el ViewModel a la vista
            return View(model);
        }
    }
}
