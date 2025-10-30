using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Services.Interfaces
{
    public interface IOfertaService
    {
        Task<List<ProductoOfertaViewModel>> ObtenerProductosEnOferta();
        Task<Oferta> ObtenerOfertaVigentePorProducto(int idProducto);
        Task<bool> ProductoTieneOfertaVigente(int idProducto);
        Task<decimal?> ObtenerPrecioOferta(int idProducto);
        Task<List<Oferta>> ObtenerOfertasProximasAVencer(int dias = 7);
        Task<decimal?> ObtenerPrecioConOferta(int idProducto);
    }
}