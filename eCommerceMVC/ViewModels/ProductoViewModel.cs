using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace eCommerceMVC.ViewModels
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        [Required]
        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        public int? IdMarca { get; set; }

        [Required]
        public int? IdCategoria { get; set; }

        public decimal? Precio { get; set; }

        public int? Stock { get; set; }

        // Solo para la vista: imagen opcional
        public IFormFile? ImagenArchivo { get; set; }

        public string? RutaImagen { get; set; }
        public string? NombreImagen { get; set; }

        public bool Activo { get; set; } = true; // valor por defecto si querés

        public DateTime? FechaRegistro { get; set; }
    }
}
