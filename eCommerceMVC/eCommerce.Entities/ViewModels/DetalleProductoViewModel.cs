using Microsoft.AspNetCore.Mvc;
using eCommerce.Services.Interfaces;
using System.Threading.Tasks;

namespace eCommerce.Entities.ViewModels
{
    public class DetalleProductoViewModel
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public string? RutaImagen { get; set; }
    }
}
