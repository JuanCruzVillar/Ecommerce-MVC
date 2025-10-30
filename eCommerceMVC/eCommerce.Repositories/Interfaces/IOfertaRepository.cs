using eCommerce.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Repositories.Interfaces
{
    public interface IOfertaRepository
    {
        Task<List<Oferta>> ObtenerOfertasVigentesAsync();
        Task<Oferta> ObtenerOfertaPorProductoAsync(int idProducto);
        Task<Dictionary<int, Oferta>> ObtenerOfertasVigentesDiccionarioAsync();
    }
}