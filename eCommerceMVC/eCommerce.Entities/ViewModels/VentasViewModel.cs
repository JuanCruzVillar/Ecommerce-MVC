namespace eCommerce.Entities.ViewModels
{
    public class VentasViewModel
    {
        public Dashboard Resumen { get; set; }
        public List<VentaDetalleViewModel> Detalles { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int EstadoSeleccionado { get; set; }
    }
}