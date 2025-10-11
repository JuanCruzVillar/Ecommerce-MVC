using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;


namespace eCommerce.Services.Implementations
{
    public class ArmatuPcService : IArmatuPcService
    {
        private readonly DbecommerceContext _context;

        public ArmatuPcService(DbecommerceContext context)
        {
            _context = context;
        }

        // Obtener procesadores de una marca específica (AMD Ryzen o Intel)
        public async Task<List<ProductoDTO>> ObtenerProcesadoresAsync(string marca)
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Procesador") &&
                           p.IdMarcaNavigation.Descripcion == marca)
                .ToListAsync();

            return MapearProductosADTO(productos);
        }

        // Obtener motherboards compatibles con la marca del procesador
        public async Task<List<ProductoDTO>> ObtenerMotherboardsAsync(string marca)
        {
            var socket = marca == "AMD" ? "Socket AMD" : "Socket Intel";

            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           p.IdCategoriaNavigation.Descripcion == socket)
                .ToListAsync();

            return MapearProductosADTO(productos);
        }

        // Obtener memorias RAM (genéricas)
        public async Task<List<ProductoDTO>> ObtenerRamsAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           p.IdCategoriaNavigation.Descripcion.Contains("Memoria RAM"))
                .ToListAsync();

            return MapearProductosADTO(productos);
        }

        // Obtener tarjetas gráficas
        public async Task<List<ProductoDTO>> ObtenerGpusAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           p.IdCategoriaNavigation.Descripcion.Contains("Tarjeta Gráfica"))
                .ToListAsync();

            return MapearProductosADTO(productos);
        }

        // Obtener almacenamiento (SSD, HDD)
        public async Task<List<ProductoDTO>> ObtenerAlmacenamientoAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (p.IdCategoriaNavigation.Descripcion.Contains("SSD") ||
                            p.IdCategoriaNavigation.Descripcion.Contains("HDD")))
                .ToListAsync();

            return MapearProductosADTO(productos);
        }

        // Obtener fuentes de poder
        public async Task<List<ProductoDTO>> ObtenerPsusAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           p.IdCategoriaNavigation.Descripcion.Contains("Fuente"))
                .ToListAsync();

            return MapearProductosADTO(productos);
        }

        // Obtener coolers (opcionales)
        public async Task<List<ProductoDTO>> ObtenerCoolersAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           p.IdCategoriaNavigation.Descripcion.Contains("Cooler"))
                .ToListAsync();

            return MapearProductosADTO(productos);
        }

        // Obtener producto detallado
        public async Task<ProductoDTO> ObtenerProductoDetalladoAsync(int idProducto)
        {
            var producto = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto && p.Activo == true);

            if (producto == null) return null;

            return MapearProductoADTO(producto);
        }

        // Guardar configuración en base de datos
        public async Task<bool> GuardarConfiguracionAsync(int idCliente, GuardarConfiguracionViewModel model)
        {
            try
            {
                var configuracion = new ConfiguracionPc
                {
                    IdCliente = idCliente,
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Total = model.Configuracion.Total,
                    FechaCreacion = DateTime.Now,
                    Activo = true
                };

                _context.ConfiguracionesPc.Add(configuracion);
                await _context.SaveChangesAsync();

               
                var detalles = new List<ConfiguracionPcDetalle>();

                if (model.Configuracion.IdProcesadorSeleccionado.HasValue)
                    detalles.Add(new ConfiguracionPcDetalle
                    {
                        IdConfiguracion = configuracion.IdConfiguracion,
                        IdProducto = model.Configuracion.IdProcesadorSeleccionado.Value,
                        Tipo = "Procesador",
                        Cantidad = 1,
                        PrecioUnitario = model.Configuracion.ProcesadorSeleccionadoInfo.PrecioUnitario,
                        Subtotal = model.Configuracion.ProcesadorSeleccionadoInfo.PrecioUnitario
                    });

                if (model.Configuracion.IdMotherboardSeleccionado.HasValue)
                    detalles.Add(new ConfiguracionPcDetalle
                    {
                        IdConfiguracion = configuracion.IdConfiguracion,
                        IdProducto = model.Configuracion.IdMotherboardSeleccionado.Value,
                        Tipo = "Motherboard",
                        Cantidad = 1,
                        PrecioUnitario = model.Configuracion.MotherboardSeleccionadoInfo.PrecioUnitario,
                        Subtotal = model.Configuracion.MotherboardSeleccionadoInfo.PrecioUnitario
                    });

                foreach (var componente in model.Configuracion.ComponentesSeleccionados)
                {
                    detalles.Add(new ConfiguracionPcDetalle
                    {
                        IdConfiguracion = configuracion.IdConfiguracion,
                        IdProducto = componente.IdProducto,
                        Tipo = componente.Tipo,
                        Cantidad = componente.Cantidad,
                        PrecioUnitario = componente.PrecioUnitario,
                        Subtotal = componente.Subtotal
                    });
                }

                _context.ConfiguracionesPcDetalles.AddRange(detalles);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar configuración: {ex.Message}");
                return false;
            }
        }

       
        public async Task<List<ConfiguracionGuardadaDTO>> ObtenerConfiguracionesGuardasAsync(int idCliente)
        {
            var configuraciones = await _context.ConfiguracionesPc
                .Include(c => c.ConfiguracionesPcDetalles)
                .ThenInclude(d => d.IdProductoNavigation)
                .ThenInclude(p => p.IdMarcaNavigation)
                .Where(c => c.IdCliente == idCliente && c.Activo == true)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();

            return configuraciones.Select(c => new ConfiguracionGuardadaDTO
            {
                IdConfiguracion = c.IdConfiguracion,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Total = c.Total ?? 0,
                FechaCreacion = c.FechaCreacion ?? DateTime.Now,
                TotalComponentes = c.ConfiguracionesPcDetalles.Count,
                Componentes = c.ConfiguracionesPcDetalles.Select(d => new ComponenteSeleccionadoDTO
                {
                    IdProducto = d.IdProducto ?? 0,
                    Nombre = d.IdProductoNavigation.Nombre,
                    Tipo = d.Tipo,
                    Marca = d.IdProductoNavigation.IdMarcaNavigation.Descripcion,
                    PrecioUnitario = d.PrecioUnitario ?? 0,
                    Cantidad = d.Cantidad ?? 1,
                    RutaImagen = d.IdProductoNavigation.RutaImagen
                }).ToList()
            }).ToList();
        }

       
        public async Task<ConfiguracionGuardadaDTO> ObtenerConfiguracionGuardadaAsync(int idConfiguracion, int idCliente)
        {
            var configuracion = await _context.ConfiguracionesPc
                .Include(c => c.ConfiguracionesPcDetalles)
                .ThenInclude(d => d.IdProductoNavigation)
                .ThenInclude(p => p.IdMarcaNavigation)
                .FirstOrDefaultAsync(c => c.IdConfiguracion == idConfiguracion && c.IdCliente == idCliente);

            if (configuracion == null) return null;

            return new ConfiguracionGuardadaDTO
            {
                IdConfiguracion = configuracion.IdConfiguracion,
                Nombre = configuracion.Nombre,
                Descripcion = configuracion.Descripcion,
                Total = configuracion.Total ?? 0,
                FechaCreacion = configuracion.FechaCreacion ?? DateTime.Now,
                TotalComponentes = configuracion.ConfiguracionesPcDetalles.Count,
                Componentes = configuracion.ConfiguracionesPcDetalles.Select(d => new ComponenteSeleccionadoDTO
                {
                    IdProducto = d.IdProducto ?? 0,
                    Nombre = d.IdProductoNavigation.Nombre,
                    Tipo = d.Tipo,
                    Marca = d.IdProductoNavigation.IdMarcaNavigation.Descripcion,
                    PrecioUnitario = d.PrecioUnitario ?? 0,
                    Cantidad = d.Cantidad ?? 1,
                    RutaImagen = d.IdProductoNavigation.RutaImagen
                }).ToList()
            };
        }

        
        public async Task<bool> EliminarConfiguracionAsync(int idConfiguracion, int idCliente)
        {
            try
            {
                var configuracion = await _context.ConfiguracionesPc
                    .FirstOrDefaultAsync(c => c.IdConfiguracion == idConfiguracion && c.IdCliente == idCliente);

                if (configuracion == null) return false;

                configuracion.Activo = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

       
        public async Task<decimal> CalcularTotalAsync(ArmaPcViewModel configuracion)
        {
            decimal total = 0;

            if (configuracion.ProcesadorSeleccionadoInfo != null)
                total += configuracion.ProcesadorSeleccionadoInfo.Subtotal;

            if (configuracion.MotherboardSeleccionadoInfo != null)
                total += configuracion.MotherboardSeleccionadoInfo.Subtotal;

            foreach (var componente in configuracion.ComponentesSeleccionados)
                total += componente.Subtotal;

            return total;
        }

       
        public async Task<ArmaPcViewModel> ObtenerConfiguracionActualAsync(int idCliente)
        {
            
            return new ArmaPcViewModel { Paso = 1 };
        }

       
        private List<ProductoDTO> MapearProductosADTO(List<Producto> productos)
        {
            return productos.Select(p => MapearProductoADTO(p)).ToList();
        }

        private ProductoDTO MapearProductoADTO(Producto producto)
        {
            return new ProductoDTO
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio ?? 0,
                Stock = producto.Stock ?? 0,
                RutaImagen = producto.RutaImagen,
                NombreImagen = producto.NombreImagen,
                Marca = producto.IdMarcaNavigation?.Descripcion,
                Categoria = producto.IdCategoriaNavigation?.Descripcion,
                CategoriaPadre = producto.IdCategoriaNavigation?.CategoriaPadre?.Descripcion,
                Especificaciones = producto.ProductoEspecificaciones
                    .Where(e => e.Activo == true)
                    .OrderBy(e => e.Orden)
                    .Select(e => new EspecificacionProductoDTO { Clave = e.Clave, Valor = e.Valor })
                    .ToList()
            };
        }
    }
}