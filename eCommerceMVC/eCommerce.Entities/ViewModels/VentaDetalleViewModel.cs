namespace eCommerce.Entities.ViewModels
{
    public class VentaDetalleViewModel
    {
        
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }

        public int IdCliente { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteCorreo { get; set; }

        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; }
        public decimal Precio { get; set; }
        
        public int TotalProductos { get; set; }
        public decimal ImporteTotal { get; set; }
       
        public string IdTransaccion { get; set; }
        public string EstadoPedido { get; set; }
        public string MetodoPago { get; set; }
        public decimal DescuentoAplicado { get; set; }
        public decimal CostoEnvio { get; set; }
    }
}