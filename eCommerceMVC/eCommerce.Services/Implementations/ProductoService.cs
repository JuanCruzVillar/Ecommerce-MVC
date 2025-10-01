using eCommerce.Entities;
using eCommerce.Repositories;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;
using eCommerce.Entities.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _productoRepository.GetAllAsync();
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            return await _productoRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Producto producto)
        {
            await _productoRepository.AddAsync(producto);
            return true;
        }

        public async Task<bool> UpdateAsync(Producto producto)
        {
            await _productoRepository.UpdateAsync(producto);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null) return false;

            await _productoRepository.DeleteAsync(id);
            return true;
        }

       
        public async Task<IEnumerable<Producto>> GetAllWithCategoriasAsync()
        {
            return await _productoRepository.GetAllWithCategoriasAsync();
        }

        public async Task<Producto> GetByIdWithEspecificacionesAsync(int id)
        {
            return await _productoRepository.GetByIdWithEspecificacionesAsync(id);
        }

        public async Task<Producto> GetByIdWithImagenesAsync(int id)
        {
            return await _productoRepository.GetByIdWithImagenesAsync(id);
        }

        public async Task<bool> AgregarImagenAsync(ProductoImagen imagen)
        {
            return await _productoRepository.AgregarImagenAsync(imagen);
        }

        public async Task<bool> EliminarImagenAsync(int idImagen)
        {
            return await _productoRepository.EliminarImagenAsync(idImagen);
        }

        public async Task<bool> MarcarImagenPrincipalAsync(int idImagen, int idProducto)
        {
            return await _productoRepository.MarcarImagenPrincipalAsync(idImagen, idProducto);
        }

        public async Task<bool> ReordenarImagenesAsync(List<OrdenImagenDto> orden)
        {
            return await _productoRepository.ReordenarImagenesAsync(orden);
        }

        public async Task<CatalogoFiltrosViewModel> BuscarProductosConFiltrosAsync(
    string busqueda,
    int? categoriaId,
    int? marcaId,
    decimal? precioMin,
    decimal? precioMax,
    string ordenamiento)
        {
            // Obtener productos con filtros
            var (productos, total) = await _productoRepository.BuscarConFiltrosAsync(
                busqueda, categoriaId, marcaId, precioMin, precioMax, ordenamiento
            );

            // Obtener datos para los filtros
            var rangoPrecio = await _productoRepository.ObtenerRangoPreciosAsync();
            var categorias = await _productoRepository.ObtenerCategoriasConConteoAsync();
            var marcas = await _productoRepository.ObtenerMarcasConConteoAsync();

            var viewModel = new CatalogoFiltrosViewModel
            {
                Productos = productos.Select(p => new DetalleProductoViewModel
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre ?? "Sin nombre",
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    RutaImagen = p.RutaImagen,
                    IdCategoria = p.IdCategoria
                }).ToList(),

                BusquedaTexto = busqueda,
                CategoriaId = categoriaId,
                MarcaId = marcaId,
                PrecioMin = precioMin,
                PrecioMax = precioMax,
                Ordenamiento = ordenamiento,

                Categorias = categorias.Select(c => new FiltroItem
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Cantidad = c.Cantidad
                }).ToList(),

                Marcas = marcas.Select(m => new FiltroItem
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Cantidad = m.Cantidad
                }).ToList(),

                PrecioMinimoDisponible = rangoPrecio.min,
                PrecioMaximoDisponible = rangoPrecio.max,
                TotalResultados = total
            };

            return viewModel;
        }
    }
}
