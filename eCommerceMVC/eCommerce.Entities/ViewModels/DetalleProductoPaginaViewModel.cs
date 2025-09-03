using eCommerce.Entities.ViewModels;

namespace eCommerce.Entities.ViewModels
{
    public class DetalleProductoPaginaViewModel
    {
        
            public DetalleProductoViewModel Producto { get; set; }
            public List<DetalleProductoViewModel> Relacionados { get; set; } = new();
        
    }
}
