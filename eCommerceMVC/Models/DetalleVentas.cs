using System;
using System.Collections.Generic;

namespace eCommerceMVC.Models;

public partial class DetalleVentas
{
    public int IdDetalleVenta { get; set; }

    public int? IdVenta { get; set; }

    public int? IdProducto { get; set; }

    public int? Cantidad { get; set; }

    public decimal? Total { get; set; }

    public virtual Productos? IdProductoNavigation { get; set; }

    public virtual Ventas? IdVentaNavigation { get; set; }
}
