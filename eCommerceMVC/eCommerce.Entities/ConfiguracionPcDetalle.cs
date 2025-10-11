using eCommerce.Entities;

namespace eCommerce.Entities

{
    public class ConfiguracionPcDetalle
{
    public int IdDetalleConfiguracion { get; set; }
    public int IdConfiguracion { get; set; }
    public int? IdProducto { get; set; }
    public string Tipo { get; set; }
    public int? Cantidad { get; set; }
    public decimal? PrecioUnitario { get; set; }
    public decimal? Subtotal { get; set; }
    public DateTime? FechaAgregado { get; set; }

    // Relaciones
    public virtual ConfiguracionPc IdConfiguracionNavigation { get; set; }
    public virtual Producto IdProductoNavigation { get; set; }
}
}