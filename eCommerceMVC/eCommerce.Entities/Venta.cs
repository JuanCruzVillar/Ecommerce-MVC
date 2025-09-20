using eCommerceMVC.eCommerce.Entities;
using System;
using System.Collections.Generic;

namespace eCommerce.Entities;

public partial class Venta
{
    public int IdVenta { get; set; }

    public int? IdCliente { get; set; }

    public int? TotalProductos { get; set; }

    public decimal? ImporteTotal { get; set; }

    public string? Contacto { get; set; }

    public string? IdProvincia { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string? IdTransaccion { get; set; }

    public DateTime? FechaVenta { get; set; }

    public virtual ICollection<DetalleVentas> DetalleVenta { get; set; } = new List<DetalleVentas>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public int? IdDireccionEnvio { get; set; }
    public int? IdMetodoPago { get; set; }
    public int? IdEstadoPedido { get; set; } = 1; // Por defecto "Pendiente"
    public int? IdCupon { get; set; }
    public decimal? DescuentoAplicado { get; set; } = 0;
    public decimal? CostoEnvio { get; set; } = 0;
    public string? NotasEspeciales { get; set; } // Comentarios del cliente
    public DateTime? FechaEstimadaEntrega { get; set; }

    // Nuevas navegaciones
    public virtual DireccionEnvio? IdDireccionEnvioNavigation { get; set; }
    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }
    public virtual EstadoPedido? IdEstadoPedidoNavigation { get; set; }
    public virtual Cupon? IdCuponNavigation { get; set; }
    public virtual ICollection<HistorialPedido> HistorialPedidos { get; set; } = new List<HistorialPedido>();
}
