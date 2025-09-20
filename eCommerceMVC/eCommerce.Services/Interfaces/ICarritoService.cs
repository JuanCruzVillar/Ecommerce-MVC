using eCommerce.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Services.Interfaces
{
    public interface ICarritoService
    {
        // Métodos existentes
        Task<List<Carrito>> ObtenerCarritoAsync(int? clienteId);
        Task AgregarProductoAsync(int? clienteId, int productoId, int cantidad);
        Task EliminarProductoAsync(int? clienteId, int productoId);
        Task VaciarCarritoAsync(int? clienteId);

        // Métodos adicionales para el checkout
        Task<List<CarritoItemViewModel>> ObtenerItemsCarritoAsync(int idCliente);
        Task<int> ObtenerCantidadItemsAsync(int idCliente);
        Task<decimal> ObtenerTotalCarritoAsync(int idCliente);
        Task<bool> ValidarStockDisponibleAsync(int idCliente, int idProducto, int cantidad);
        Task<bool> ActualizarCantidadAsync(int idCliente, int idProducto, int nuevaCantidad);
    }
}