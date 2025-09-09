//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace eCommerce.Entities.ViewModels
//{
//    // ==========================
//    // Dashboard y Ventas
//    // ==========================

//    public class Dashboard
//    {
//        public decimal TotalVenta { get; set; }
//        public int TotalCliente { get; set; }
//        public int TotalProducto { get; set; }
//    }

//    public class DashboardViewModel
//    {
//        public Dashboard Resumen { get; set; }
//        public List<VentaDetalleViewModel> Ventas { get; set; } = new();
//    }

//    public class VentaDetalleViewModel
//    {
//        public int IdVenta { get; set; }
//        public DateTime FechaVenta { get; set; }
//        public int IdCliente { get; set; }
//        public string ClienteNombre { get; set; } = string.Empty;
//        public decimal ImporteTotal { get; set; }
//        public string IdTransaccion { get; set; } = string.Empty;
//        public string Estado { get; set; } = "Pendiente";
//        public List<DetalleProductoVentaViewModel> Detalles { get; set; } = new();
//    }

//    public class DetalleProductoVentaViewModel
//    {
//        public int IdProducto { get; set; }
//        public string ProductoNombre { get; set; } = string.Empty;
//        public decimal Precio { get; set; }
//        public int Cantidad { get; set; }
//        public decimal Total => Precio * Cantidad;
//    }

//    // ==========================
//    // Catálogo / Productos
//    // ==========================

//    public class CatalogoViewModel
//    {
//        public List<DetalleProductoViewModel> Productos { get; set; } = new();
//        public List<CategoriaViewModel> Categorias { get; set; } = new();
//        public int? CategoriaSeleccionada { get; set; }
//    }

//    public class DetalleProductoViewModel
//    {
//        public int IdProducto { get; set; }
//        public string Nombre { get; set; } = string.Empty;
//        public string? Descripcion { get; set; }
//        public decimal? Precio { get; set; }
//        public string? RutaImagen { get; set; }
//        public int? IdCategoria { get; set; }
//        public int? IdSubcategoria { get; set; }
//        public int Stock { get; set; }
//        public IFormFile? ImagenArchivo { get; set; }
//        public int? IdMarca { get; set; }
//        public bool? Activo { get; set; }
//    }

//    public class DetalleProductoPaginaViewModel
//    {
//        public DetalleProductoViewModel Producto { get; set; }
//        public List<DetalleProductoViewModel> Relacionados { get; set; } = new();
//    }

//    public class ClienteViewModel
//    {
//        [Required(ErrorMessage = "El nombre es obligatorio")]
//        public string Nombres { get; set; } = null!;

//        [Required(ErrorMessage = "El apellido es obligatorio")]
//        public string Apellidos { get; set; } = null!;

//        [Required(ErrorMessage = "El correo es obligatorio")]
//        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
//        public string Correo { get; set; } = null!;

//        [Required(ErrorMessage = "La contraseña es obligatoria")]
//        [DataType(DataType.Password)]
//        public string Contraseña { get; set; } = null!;
//    }
//    // ==========================
//    // Categorías y Subcategorías
//    // ==========================

//    public class CategoriaViewModel
//    {
//        public int IdCategoria { get; set; }
//        public string Nombre { get; set; } = string.Empty;
//        public List<SubcategoriaViewModel> Subcategorias { get; set; } = new();
//    }

//    public class SubcategoriaViewModel
//    {
//        public int IdSubcategoria { get; set; }
//        public string Nombre { get; set; } = string.Empty;
//    }

//    public class ProductoViewModel : DetalleProductoViewModel
//    {
//        public int? Stock { get; set; }
//        public IFormFile? ImagenArchivo { get; set; }
//        public DateTime? FechaRegistro { get; set; }
//        public string NombreImagen { get; set; } = string.Empty;
//        public bool Activo { get; set; } = true;
//    }
//}
