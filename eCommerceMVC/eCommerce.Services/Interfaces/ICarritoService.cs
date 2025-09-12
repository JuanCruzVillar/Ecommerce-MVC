using eCommerce.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Services.Interfaces
{
    public interface ICarritoService
    {
        Task<List<Carrito>> ObtenerCarritoAsync(int? clienteId);
        Task AgregarProductoAsync(int? clienteId, int productoId, int cantidad);
        Task EliminarProductoAsync(int? clienteId, int productoId);
        Task VaciarCarritoAsync(int? clienteId);
    }
}
