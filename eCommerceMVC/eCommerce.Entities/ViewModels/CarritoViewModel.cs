using Microsoft.AspNetCore.Mvc;
using eCommerce.Services.Interfaces;
using System.Threading.Tasks;


namespace eCommerce.Entities.ViewModels
{
    public class CarritoViewModel
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? RutaImagen { get; set; }
        public decimal? Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio.GetValueOrDefault() * Cantidad;
    }
}




