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
}
