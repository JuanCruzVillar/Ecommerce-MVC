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

        private readonly ILogger<ArmatuPcService> _logger;

        public ArmatuPcService(DbecommerceContext context, ILogger<ArmatuPcService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtener memorias RAM (Categorías: DDR4, DDR5 - Padre: Memoria Ram)
        public async Task<List<ProductoDTO>> ObtenerRamsAsync()
        {
            // IdCategoria 13 (DDR4) y 14 (DDR5) - Padre: 11 (Memoria Ram)
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (
                               // Buscar por IdCategoria directamente (más preciso)
                               p.IdCategoria == 13 || // DDR4
                               p.IdCategoria == 14 || // DDR5
                                                      // O por categoría padre "Memoria Ram"
                               (p.IdCategoriaNavigation.IdCategoriaPadre == 11)
                           ))
                .ToListAsync();

            _logger.LogDebug("Encontradas {Count} memorias RAM disponibles", productos.Count);

            return MapearProductosADTO(productos);
        }

        // Obtener tarjetas gráficas (búsqueda flexible)
        public async Task<List<ProductoDTO>> ObtenerGpusAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (
                               // Buscar por múltiples variantes
                               p.IdCategoriaNavigation.Descripcion.Contains("Gráfica") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Grafica") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("GPU") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Video") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Placa") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Tarjeta") ||
                               // O categoría padre
                               (p.IdCategoriaNavigation.CategoriaPadre != null &&
                                (p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Gráfica") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("GPU") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Video") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Placa")))
                           ))
                .ToListAsync();

           

            return MapearProductosADTO(productos);
        }

        // Obtener procesadores de una marca específica (AMD o Intel)
        public async Task<List<ProductoDTO>> ObtenerProcesadoresAsync(string marca)
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           p.IdMarcaNavigation.Descripcion == marca &&
                           (p.IdCategoriaNavigation.Descripcion.Contains("Procesador") ||
                            p.IdCategoriaNavigation.Descripcion.Contains("CPU") ||
                            (p.IdCategoriaNavigation.CategoriaPadre != null &&
                             (p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Procesador") ||
                              p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("CPU")))))
                .ToListAsync();

           
            return MapearProductosADTO(productos);
        }

        // Obtener motherboards compatibles con la marca del procesador
        public async Task<List<ProductoDTO>> ObtenerMotherboardsAsync(string marca)
        {
            var socket = marca == "AMD" ? "AMD" : "Intel";

            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (
                               // Buscar por categoría
                               p.IdCategoriaNavigation.Descripcion.Contains("Motherboard") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Mother") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Placa Base") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Placa Madre") ||
                               // O por categoría padre
                               (p.IdCategoriaNavigation.CategoriaPadre != null &&
                                (p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Motherboard") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Mother")))
                           ) &&
                           // Que sea compatible con el socket
                           (p.IdCategoriaNavigation.Descripcion.Contains(socket) ||
                            p.Nombre.Contains(socket) ||
                            (p.IdCategoriaNavigation.CategoriaPadre != null &&
                             p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains(socket))))
                .ToListAsync();

           
            return MapearProductosADTO(productos);
        }

        // Obtener almacenamiento (SSD, HDD)
        public async Task<List<ProductoDTO>> ObtenerAlmacenamientoAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (
                               p.IdCategoriaNavigation.Descripcion.Contains("SSD") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("HDD") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Disco") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Almacenamiento") ||
                               (p.IdCategoriaNavigation.CategoriaPadre != null &&
                                (p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Almacenamiento") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Disco")))
                           ))
                .ToListAsync();

           
            return MapearProductosADTO(productos);
        }

        // Obtener fuentes de poder
        public async Task<List<ProductoDTO>> ObtenerPsusAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (
                               p.IdCategoriaNavigation.Descripcion.Contains("Fuente") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("PSU") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Power Supply") ||
                               (p.IdCategoriaNavigation.CategoriaPadre != null &&
                                (p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Fuente") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("PSU")))
                           ))
                .ToListAsync();

           
            return MapearProductosADTO(productos);
        }

        // Obtener coolers (opcionales)
        public async Task<List<ProductoDTO>> ObtenerCoolersAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (
                               p.IdCategoriaNavigation.Descripcion.Contains("Cooler") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Refrigeración") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Refrigeracion") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Ventilador") ||
                               (p.IdCategoriaNavigation.CategoriaPadre != null &&
                                (p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Cooler") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Refrigeración")))
                           ))
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
        

        public async Task<List<ProductoDTO>> ObtenerGabinetesAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ThenInclude(c => c.CategoriaPadre)
                .Include(p => p.ProductoEspecificaciones)
                .Where(p => p.Activo == true &&
                           p.Stock > 0 &&
                           (
                               p.IdCategoriaNavigation.Descripcion.Contains("Gabinete") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Case") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Caja") ||
                               p.IdCategoriaNavigation.Descripcion.Contains("Chasis") ||
                               (p.IdCategoriaNavigation.CategoriaPadre != null &&
                                (p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Gabinete") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Case") ||
                                 p.IdCategoriaNavigation.CategoriaPadre.Descripcion.Contains("Caja")))
                           ))
                .ToListAsync();

            return MapearProductosADTO(productos);
        }
    }
}