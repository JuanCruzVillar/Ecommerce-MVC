using eCommerceMVC.eCommerce.Entities;

namespace eCommerce.Entities
{
    public class ConfiguracionPc
    {
        public int IdConfiguracion { get; set; }
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? Total { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool? Activo { get; set; }

        // Relaciones
        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual ICollection<ConfiguracionPcDetalle> ConfiguracionesPcDetalles { get; set; } = new List<ConfiguracionPcDetalle>();
    }

}