using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Services.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetAllWithCategoriasAsync();
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto> GetByIdAsync(int id);
        Task<bool> CreateAsync(Producto producto);
        Task<bool> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);

        Task<Producto> GetByIdWithEspecificacionesAsync(int id);

        Task<Producto> GetByIdWithImagenesAsync(int id);

        Task<int> GetCantidadPorMarcaAsync(int idMarca);

        Task<bool> AgregarImagenAsync(ProductoImagen imagen);
        Task<bool> EliminarImagenAsync(int idImagen);
        Task<bool> MarcarImagenPrincipalAsync(int idImagen, int idProducto);
        Task<bool> ReordenarImagenesAsync(List<OrdenImagenDto> orden);

        Task<CatalogoFiltrosViewModel> BuscarProductosConFiltrosAsync(
    string busqueda,
    int? categoriaId,
    int? marcaId,
    decimal? precioMin,
    decimal? precioMax,
    string ordenamiento
);
    }
}
