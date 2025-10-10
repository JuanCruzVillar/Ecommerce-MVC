namespace eCommerce.Entities.ViewModels
{
    public class Dashboard
    {
        public decimal TotalVenta { get; set; }
        public int TotalCliente { get; set; }
        public int TotalProducto { get; set; }
        public int VentasHoy { get; set; }
        public decimal TicketPromedio { get; set; }
    }
}