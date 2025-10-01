using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Repositories.Interfaces
{
    public interface IProductoRepository
    {

        Task<IEnumerable<Producto>> GetAllWithCategoriasAsync();

        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto> GetByIdAsync(int id);
        Task AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);

        Task<Producto> GetByIdWithEspecificacionesAsync(int id);

        Task<Producto> GetByIdWithImagenesAsync(int id);

        Task<bool> AgregarImagenAsync(ProductoImagen imagen);
        Task<bool> EliminarImagenAsync(int idImagen);
        Task<bool> MarcarImagenPrincipalAsync(int idImagen, int idProducto);
        Task<bool> ReordenarImagenesAsync(List<OrdenImagenDto> orden);

        Task<(List<Producto> productos, int total)> BuscarConFiltrosAsync(
    string busqueda,
    int? categoriaId,
    int? marcaId,
    decimal? precioMin,
    decimal? precioMax,
    string ordenamiento
);

        Task<(decimal min, decimal max)> ObtenerRangoPreciosAsync();
        Task<List<FiltroConteo>> ObtenerCategoriasConConteoAsync();
        Task<List<FiltroConteo>> ObtenerMarcasConConteoAsync();

        public class FiltroConteo
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public int Cantidad { get; set; }
        }
    }
}
