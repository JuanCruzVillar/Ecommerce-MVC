using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Repositories.Implementations
{
    public class OfertaRepository : IOfertaRepository
    {
        private readonly DbecommerceContext _context;

        public OfertaRepository(DbecommerceContext context)
        {
            _context = context;
        }

        public async Task<List<Oferta>> ObtenerOfertasVigentesAsync()
        {
            var ahora = DateTime.Now;
            return await _context.Ofertas
                .Where(o => o.Activo
                    && o.FechaInicio <= ahora
                    && o.FechaFin >= ahora)
                .ToListAsync();
        }

        public async Task<Oferta> ObtenerOfertaPorProductoAsync(int idProducto)
        {
            var ahora = DateTime.Now;
            return await _context.Ofertas
                .Where(o => o.IdProducto == idProducto
                    && o.Activo
                    && o.FechaInicio <= ahora
                    && o.FechaFin >= ahora)
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<int, Oferta>> ObtenerOfertasVigentesDiccionarioAsync()
        {
            var ofertas = await ObtenerOfertasVigentesAsync();
            return ofertas.ToDictionary(o => o.IdProducto, o => o);
        }
    }
}