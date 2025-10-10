
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Services.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDetalleViewModel>> ObtenerTodasLasVentasAsync();
        Task<VentaDetalleViewModel> ObtenerVentaPorIdAsync(int idVenta);
        Task<Dashboard> ObtenerEstadisticasDashboardAsync();
    }
}