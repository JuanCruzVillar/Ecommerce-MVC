using eCommerce.Entities.ViewModels;

namespace eCommerce.Entities.ViewModels
{
    public class DetalleProductoPaginaViewModel
    {
        public DetalleProductoViewModel Producto { get; set; }
        public List<DetalleProductoViewModel> Relacionados { get; set; } = new();
        public List<ProductoEspecificacionViewModel> Especificaciones { get; set; } = new();
        public List<ProductoImagenViewModel> Imagenes { get; set; } = new();
    }

    public class ProductoEspecificacionViewModel
    {
        public string Clave { get; set; }
        public string Valor { get; set; }
        public int Orden { get; set; }
    }

    public class ProductoImagenViewModel
    {
        public int IdImagen { get; set; }
        public string RutaImagen { get; set; }
        public bool EsPrincipal { get; set; }
        public int Orden { get; set; }
    }
}