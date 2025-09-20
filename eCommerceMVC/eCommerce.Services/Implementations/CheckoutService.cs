using eCommerce.Entities;
using eCommerce.Data;
using eCommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Implementations
{
    public class CheckoutService : ICheckoutService
    {
        private readonly DbecommerceContext _context;
        private readonly ICarritoService _carritoService;

        public CheckoutService(DbecommerceContext context, ICarritoService carritoService)
        {
            _context = context;
            _carritoService = carritoService;
        }

        public async Task<CheckoutViewModel> ObtenerCheckoutAsync(int idCliente)
        {
            var checkout = new CheckoutViewModel();

          
            checkout.ItemsCarrito = await _carritoService.ObtenerItemsCarritoAsync(idCliente);

           
            checkout.Subtotal = checkout.ItemsCarrito.Sum(x => x.Subtotal);
            checkout.TotalItems = checkout.ItemsCarrito.Sum(x => x.Cantidad);

            
            checkout.Cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

            checkout.DireccionesDisponibles = await _context.DireccionesEnvio
                .Where(d => d.IdCliente == idCliente && d.Activo)
                .OrderByDescending(d => d.EsDireccionPrincipal)
                .ThenByDescending(d => d.FechaRegistro)
                .ToListAsync();

            
            checkout.MetodosPagoDisponibles = await _context.MetodosPago
                .Where(mp => mp.Activo)
                .OrderBy(mp => mp.Nombre)
                .ToListAsync();

            
            checkout.CostoEnvio = await CalcularCostoEnvioBasicoAsync(checkout.Subtotal);
            checkout.Total = checkout.Subtotal + checkout.CostoEnvio;

            return checkout;
        }

        public async Task<CuponResultViewModel> ValidarCuponAsync(string codigoCupon, decimal subtotal)
        {
            var resultado = new CuponResultViewModel { EsValido = false };

            if (string.IsNullOrEmpty(codigoCupon))
            {
                resultado.Mensaje = "Ingrese un código de cupón";
                return resultado;
            }

            var cupon = await _context.Cupones
                .FirstOrDefaultAsync(c => c.Codigo.ToUpper() == codigoCupon.ToUpper() && c.Activo);

            if (cupon == null)
            {
                resultado.Mensaje = "El cupón no existe o no es válido";
                return resultado;
            }

           
            if (DateTime.Now < cupon.FechaInicio || DateTime.Now > cupon.FechaVencimiento)
            {
                resultado.Mensaje = "El cupón ha expirado o aún no es válido";
                return resultado;
            }

           
            if (cupon.UsosActuales >= cupon.UsosMaximos)
            {
                resultado.Mensaje = "El cupón ya alcanzó el límite de usos";
                return resultado;
            }

            
            if (subtotal < cupon.MontoMinimo)
            {
                resultado.Mensaje = $"El monto mínimo para este cupón es ${cupon.MontoMinimo:N2}";
                return resultado;
            }

            
            decimal descuento = 0;
            if (cupon.DescuentoFijo > 0)
            {
                descuento = cupon.DescuentoFijo;
                resultado.TipoDescuento = "Fijo";
            }
            else if (cupon.DescuentoPorcentaje > 0)
            {
                descuento = subtotal * (cupon.DescuentoPorcentaje / 100);
                resultado.TipoDescuento = "Porcentaje";
            }

            resultado.EsValido = true;
            resultado.DescuentoAplicado = descuento;
            resultado.Mensaje = $"¡Cupón aplicado! Descuento: ${descuento:N2}";

            return resultado;
        }

        public async Task<bool> AgregarDireccionEnvioAsync(DireccionEnvioViewModel direccionVM, int idCliente)
        {
            try
            {
                Console.WriteLine($"DEBUG - Intentando guardar dirección para cliente: {idCliente}");
                Console.WriteLine($"DEBUG - Nombre: '{direccionVM?.NombreCompleto}'");
                Console.WriteLine($"DEBUG - Dirección: '{direccionVM?.Direccion}'");
                Console.WriteLine($"DEBUG - Ciudad: '{direccionVM?.Ciudad}'");
                Console.WriteLine($"DEBUG - Provincia: '{direccionVM?.Provincia}'");
                Console.WriteLine($"DEBUG - CodigoPostal: '{direccionVM?.CodigoPostal}'");
                Console.WriteLine($"DEBUG - Telefono: '{direccionVM?.Telefono}'");

                if (direccionVM == null)
                {
                    Console.WriteLine("DEBUG - ERROR: direccionVM es null");
                    return false;
                }

                // Si es dirección principal, sacr la principal actual
                if (direccionVM.EsDireccionPrincipal)
                {
                    var direccionPrincipalActual = await _context.DireccionesEnvio
                        .Where(d => d.IdCliente == idCliente && d.EsDireccionPrincipal)
                        .FirstOrDefaultAsync();

                    if (direccionPrincipalActual != null)
                    {
                        direccionPrincipalActual.EsDireccionPrincipal = false;
                    }
                }

                var nuevaDireccion = new DireccionEnvio
                {
                    IdCliente = idCliente,
                    NombreCompleto = direccionVM.NombreCompleto,
                    Direccion = direccionVM.Direccion,
                    Referencias = direccionVM.Referencias,
                    Ciudad = direccionVM.Ciudad,
                    Provincia = direccionVM.Provincia,
                    CodigoPostal = direccionVM.CodigoPostal,
                    Telefono = direccionVM.Telefono,
                    EsDireccionPrincipal = direccionVM.EsDireccionPrincipal,
                    Activo = true,
                    FechaRegistro = DateTime.Now
                };

                Console.WriteLine($"DEBUG - Agregando dirección a contexto...");
                _context.DireccionesEnvio.Add(nuevaDireccion);

                Console.WriteLine($"DEBUG - Guardando cambios...");
                await _context.SaveChangesAsync();

                Console.WriteLine($"DEBUG - Dirección guardada exitosamente con ID: {nuevaDireccion.IdDireccionEnvio}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG - ERROR al guardar dirección: {ex.Message}");
                Console.WriteLine($"DEBUG - InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"DEBUG - Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<decimal> CalcularCostoEnvioAsync(int idDireccionEnvio)
        {
            var direccion = await _context.DireccionesEnvio
                .FirstOrDefaultAsync(d => d.IdDireccionEnvio == idDireccionEnvio);

            if (direccion == null) return 0;

            // Ejemplo: diferentes costos según provincia
            decimal costoBase = direccion.Provincia.ToUpper() switch
            {
                "BUENOS AIRES" => 2500,
                "CAPITAL FEDERAL" => 2400,
                "CORDOBA" => 3600,
                "SANTA FE" => 3550,
                _ => 3700 // Otras provincias
            };

            return costoBase;
        }

        private async Task<decimal> CalcularCostoEnvioBasicoAsync(decimal subtotal)
        {
            // Envío gratis para compras mayores a $25000
            if (subtotal >= 25000) return 0;

            // Costo base de envío
            return 3000;
        }

        public async Task<int> ProcesarPedidoAsync(CheckoutViewModel checkoutVM, int idCliente)
        {
            Console.WriteLine($"DEBUG - Iniciando ProcesarPedidoAsync");
            Console.WriteLine($"DEBUG - IdCliente: {idCliente}");
            Console.WriteLine($"DEBUG - UsarNuevaDireccion: {checkoutVM.UsarNuevaDireccion}");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Validar stock
                if (!await ValidarStockProductosAsync(idCliente))
                {
                    throw new InvalidOperationException("Stock insuficiente para algunos productos");
                }

                int? idDireccionFinal = null;

                // Si usa nueva dirección, crearla primero
                if (checkoutVM.UsarNuevaDireccion)
                {
                    Console.WriteLine($"DEBUG - Creando nueva dirección...");
                    var resultadoDireccion = await AgregarDireccionEnvioAsync(checkoutVM.NuevaDireccion, idCliente);

                    if (resultadoDireccion)
                    {
                        var nuevaDireccion = await _context.DireccionesEnvio
                            .Where(d => d.IdCliente == idCliente)
                            .OrderByDescending(d => d.FechaRegistro)
                            .FirstOrDefaultAsync();
                        idDireccionFinal = nuevaDireccion?.IdDireccionEnvio;
                        Console.WriteLine($"DEBUG - Dirección creada con ID: {idDireccionFinal}");
                    }
                    else
                    {
                        throw new InvalidOperationException("Error al crear la dirección de envío");
                    }
                }
                else
                {
                    idDireccionFinal = checkoutVM.DireccionEnvioSeleccionada;
                    Console.WriteLine($"DEBUG - Usando dirección existente ID: {idDireccionFinal}");
                }

                // 2. Crear la venta
                var venta = new Venta
                {
                    IdCliente = idCliente,
                    TotalProductos = checkoutVM.TotalItems,
                    ImporteTotal = checkoutVM.Total,
                    IdDireccionEnvio = idDireccionFinal,
                    IdMetodoPago = checkoutVM.MetodoPagoSeleccionado,
                    IdEstadoPedido = 2, // Pendiente (ID generado en la BD)
                    DescuentoAplicado = checkoutVM.DescuentoAplicado,
                    CostoEnvio = checkoutVM.CostoEnvio,
                    NotasEspeciales = checkoutVM.NotasEspeciales,
                    FechaVenta = DateTime.Now,
                    FechaEstimadaEntrega = DateTime.Now.AddDays(7), // 7 días por defecto
                    IdTransaccion = Guid.NewGuid().ToString("N")[..10] // ID único corto
                };

                Console.WriteLine($"DEBUG - Creando venta...");
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();
                Console.WriteLine($"DEBUG - Venta creada con ID: {venta.IdVenta}");

                // 3. Crear detalle de venta y actualizar stock
                foreach (var item in checkoutVM.ItemsCarrito)
                {
                    var detalleVenta = new DetalleVentas
                    {
                        IdVenta = venta.IdVenta,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        Total = item.Subtotal
                    };

                    _context.DetalleVentas.Add(detalleVenta);

                    // Actualizar stock
                    var producto = await _context.Productos.FindAsync(item.IdProducto);
                    if (producto != null)
                    {
                        producto.Stock = (producto.Stock ?? 0) - item.Cantidad;
                    }
                }

                // 4. Aplicar cupón si existe
                if (!string.IsNullOrEmpty(checkoutVM.CodigoCupon) && checkoutVM.CuponAplicado)
                {
                    var cupon = await _context.Cupones
                        .FirstOrDefaultAsync(c => c.Codigo.ToUpper() == checkoutVM.CodigoCupon.ToUpper());

                    if (cupon != null)
                    {
                        cupon.UsosActuales++;
                        venta.IdCupon = cupon.IdCupon;
                    }
                }

                // 5. Limpiar carrito
                var itemsCarrito = await _context.Carritos
                    .Where(c => c.IdUsuario == idCliente)
                    .ToListAsync();
                _context.Carritos.RemoveRange(itemsCarrito);

                // 6. Crear historial de pedido
                var historial = new HistorialPedido
                {
                    IdVenta = venta.IdVenta,
                    IdEstadoPedido = 2, 
                    Comentarios = "Pedido creado exitosamente",
                    FechaCambio = DateTime.Now
                };
                _context.HistorialPedidos.Add(historial);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"DEBUG - Pedido procesado exitosamente, ID Venta: {venta.IdVenta}");
                return venta.IdVenta;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG - ERROR en ProcesarPedidoAsync: {ex.Message}");
                Console.WriteLine($"DEBUG - InnerException: {ex.InnerException?.Message}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ValidarStockProductosAsync(int idCliente)
        {
            var productosCarrito = await _context.Carritos
                .Include(c => c.IdProductoNavigation)
                .Where(c => c.IdUsuario == idCliente)
                .ToListAsync();

            foreach (var item in productosCarrito)
            {
                if (item.IdProductoNavigation?.Stock < item.Cantidad)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<PedidoConfirmacionViewModel> ObtenerConfirmacionPedidoAsync(int idVenta)
        {
            var venta = await _context.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdDireccionEnvioNavigation)
                .Include(v => v.IdMetodoPagoNavigation)
                .Include(v => v.IdEstadoPedidoNavigation)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(dv => dv.IdProductoNavigation)
                        .ThenInclude(p => p.IdMarcaNavigation)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(dv => dv.IdProductoNavigation)
                        .ThenInclude(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta == null) return null;

            var confirmacion = new PedidoConfirmacionViewModel
            {
                IdVenta = venta.IdVenta,
                NumeroVenta = $"#{venta.IdVenta:D6}",
                FechaPedido = venta.FechaVenta ?? DateTime.Now,
                Total = venta.ImporteTotal ?? 0,
                EstadoPedido = venta.IdEstadoPedidoNavigation?.Nombre ?? "Pendiente",
                DireccionEnvio = venta.IdDireccionEnvioNavigation,
                MetodoPago = venta.IdMetodoPagoNavigation,
                FechaEstimadaEntrega = venta.FechaEstimadaEntrega,
                NotasEspeciales = venta.NotasEspeciales,
                Items = venta.DetalleVenta.Select(dv => new CarritoItemViewModel
                {
                    IdProducto = dv.IdProducto ?? 0,
                    NombreProducto = dv.IdProductoNavigation?.Nombre,
                    Precio = dv.Total ?? 0 / (dv.Cantidad ?? 1),
                    Cantidad = dv.Cantidad ?? 0,
                    Subtotal = dv.Total ?? 0,
                    RutaImagen = dv.IdProductoNavigation?.RutaImagen,
                    NombreImagen = dv.IdProductoNavigation?.NombreImagen,
                    NombreMarca = dv.IdProductoNavigation?.IdMarcaNavigation?.Descripcion,
                    NombreCategoria = dv.IdProductoNavigation?.IdCategoriaNavigation?.Descripcion
                }).ToList()
            };

            return confirmacion;
        }
    }
}