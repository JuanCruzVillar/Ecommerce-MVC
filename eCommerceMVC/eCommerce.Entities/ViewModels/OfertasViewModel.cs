using System;
using System.Collections.Generic;

namespace eCommerce.Entities.ViewModels
{
    public class OfertasViewModel
    {
        public List<ProductoOfertaViewModel> ProductosEnOferta { get; set; } = new();
        public int TotalOfertas { get; set; }
        public string TituloSeccion { get; set; } = "Ofertas Especiales";
        public string DescripcionSeccion { get; set; } = "Aprovechá los mejores descuentos";
    }

    public class ProductoOfertaViewModel
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioOriginal { get; set; }
        public decimal PrecioOferta { get; set; }
        public int PorcentajeDescuento { get; set; }
        public decimal AhorroTotal { get; set; }
        public string RutaImagen { get; set; }
        public string NombreImagen { get; set; }
        public int Stock { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public DateTime FechaFinOferta { get; set; }
        public int DiasRestantes { get; set; }
        public bool EnStock => Stock > 0;

        // Helper para mostrar badge
        public string ObtenerBadgeColor()
        {
            if (PorcentajeDescuento >= 30) return "danger";
            if (PorcentajeDescuento >= 20) return "warning";
            return "info";
        }
    }
}