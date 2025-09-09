using eCommerce.Entities;
using System.Collections.Generic;

namespace eCommerce.Entities.ViewModels
{
    public class VentasViewModel
    {
        public Dashboard Resumen { get; set; }                       // Para las cards del dashboard
        public IEnumerable<VentaDetalleViewModel> Detalles { get; set; } // Para la tabla de ventas
    }
}
