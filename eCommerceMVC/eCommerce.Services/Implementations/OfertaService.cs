using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Repositories.Interfaces;
using eCommerce.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations
{
    public class OfertaService : IOfertaService
    {
        private readonly IOfertaRepository _ofertaRepository;
        private readonly IProductoRepository _productoRepository;

        public OfertaService(IOfertaRepository ofertaRepository, IProductoRepository productoRepository)
        {
            _ofertaRepository = ofertaRepository;
            _productoRepository = productoRepository;
        }

        public async Task<List<ProductoOfertaViewModel>> ObtenerProductosEnOferta()
        {
            var productos = await _productoRepository.GetAllAsync();
            var ofertas = await _ofertaRepository.ObtenerOfertasVigentesDiccionarioAsync();

            var productosEnOferta = productos
                .Where(p => ofertas.ContainsKey(p.IdProducto) && p.Activo == true && p.Stock > 0)
                .Select(p => new ProductoOfertaViewModel
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    PrecioOriginal = p.Precio.Value,
                    PrecioOferta = ofertas[p.IdProducto].PrecioOferta,
                    PorcentajeDescuento = ofertas[p.IdProducto].PorcentajeDescuento ?? 0,
                    AhorroTotal = p.Precio.Value - ofertas[p.IdProducto].PrecioOferta,
                    RutaImagen = p.RutaImagen,
                    NombreImagen = p.NombreImagen,
                    Stock = p.Stock.Value,
                    Marca = p.IdMarcaNavigation?.Descripcion,
                    Categoria = p.IdCategoriaNavigation?.Descripcion,
                    FechaFinOferta = ofertas[p.IdProducto].FechaFin,
                    DiasRestantes = (ofertas[p.IdProducto].FechaFin - DateTime.Now).Days
                })
                .OrderByDescending(p => p.PorcentajeDescuento)
                .ToList();

            return productosEnOferta;
        }

        public async Task<Oferta> ObtenerOfertaVigentePorProducto(int idProducto)
        {
            return await _ofertaRepository.ObtenerOfertaPorProductoAsync(idProducto);
        }

        public async Task<bool> ProductoTieneOfertaVigente(int idProducto)
        {
            var oferta = await ObtenerOfertaVigentePorProducto(idProducto);
            return oferta != null;
        }

        public async Task<decimal?> ObtenerPrecioOferta(int idProducto)
        {
            var oferta = await ObtenerOfertaVigentePorProducto(idProducto);
            return oferta?.PrecioOferta;
        }

        public async Task<List<Oferta>> ObtenerOfertasProximasAVencer(int dias = 7)
        {
            var ahora = DateTime.Now;
            var fechaLimite = ahora.AddDays(dias);

            var ofertas = await _ofertaRepository.ObtenerOfertasVigentesAsync();
            return ofertas
                .Where(o => o.FechaFin <= fechaLimite)
                .OrderBy(o => o.FechaFin)
                .ToList();
        }



        public async Task<decimal?> ObtenerPrecioConOferta(int idProducto)
        {
            var oferta = await ObtenerOfertaVigentePorProducto(idProducto);
            return oferta?.PrecioOferta;
        }
    }
}