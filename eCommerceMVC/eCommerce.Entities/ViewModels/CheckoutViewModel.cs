using eCommerce.Entities;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Entities
{
    public class CheckoutViewModel
    {
        // Datos del carrito
        public List<CarritoItemViewModel> ItemsCarrito { get; set; } = new List<CarritoItemViewModel>();
        public decimal Subtotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal DescuentoAplicado { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }

        // Dirección de envío
        public int? DireccionEnvioSeleccionada { get; set; }
        public List<DireccionEnvio> DireccionesDisponibles { get; set; } = new List<DireccionEnvio>();
        public DireccionEnvioViewModel NuevaDireccion { get; set; } = new DireccionEnvioViewModel();
        public bool UsarNuevaDireccion { get; set; } = false;

        // Método de pago
        public int MetodoPagoSeleccionado { get; set; }
        public List<MetodoPago> MetodosPagoDisponibles { get; set; } = new List<MetodoPago>();

        public int CantidadCuotas { get; set; } = 1;

        // Cupón de descuento
        public string CodigoCupon { get; set; }
        public bool CuponAplicado { get; set; } = false;
        public string MensajeCupon { get; set; }

        // Notas especiales
        [MaxLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres")]
        public string NotasEspeciales { get; set; }

        // Datos del cliente
        public Cliente Cliente { get; set; }

        // Confirmación
        public bool AceptaTerminos { get; set; }
    }

    // ViewModel para items del carrito
    public class CarritoItemViewModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public string RutaImagen { get; set; }
        public string NombreImagen { get; set; }
        public int StockDisponible { get; set; }
        public string NombreMarca { get; set; }
        public string NombreCategoria { get; set; }
    }

    // ViewModel para nueva dirección de envío
    public class DireccionEnvioViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [MaxLength(300, ErrorMessage = "La dirección no puede superar los 300 caracteres")]
        public string Direccion { get; set; }

        [MaxLength(100, ErrorMessage = "Las referencias no pueden superar los 100 caracteres")]
        public string Referencias { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [MaxLength(100, ErrorMessage = "La ciudad no puede superar los 100 caracteres")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "La provincia es obligatoria")]
        [MaxLength(100, ErrorMessage = "La provincia no puede superar los 100 caracteres")]
        public string Provincia { get; set; }

        [Required(ErrorMessage = "El código postal es obligatorio")]
        [MaxLength(20, ErrorMessage = "El código postal no puede superar los 20 caracteres")]
        public string CodigoPostal { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [MaxLength(50, ErrorMessage = "El teléfono no puede superar los 50 caracteres")]
        public string Telefono { get; set; }

        public bool GuardarDireccion { get; set; } = false;
        public bool EsDireccionPrincipal { get; set; } = false;
    }

    // ViewModel para aplicar cupón
    public class CuponViewModel
    {
        [Required(ErrorMessage = "Ingrese el código del cupón")]
        [MaxLength(20, ErrorMessage = "El código no puede superar los 20 caracteres")]
        public string Codigo { get; set; }
    }

    // ViewModel para respuesta de cupón
    public class CuponResultViewModel
    {
        public bool EsValido { get; set; }
        public string Mensaje { get; set; }
        public decimal DescuentoAplicado { get; set; }
        public string TipoDescuento { get; set; } // "Fijo" o "Porcentaje"
    }

    // ViewModel para confirmación de pedido
    public class PedidoConfirmacionViewModel
    {
        public int IdVenta { get; set; }
        public string NumeroVenta { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public string EstadoPedido { get; set; }
        public DireccionEnvio DireccionEnvio { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public List<CarritoItemViewModel> Items { get; set; } = new List<CarritoItemViewModel>();
        public DateTime? FechaEstimadaEntrega { get; set; }
        public string NotasEspeciales { get; set; }
    }

    // ViewModel para el resumen del pedido (sidebar)
    public class ResumenPedidoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new List<CarritoItemViewModel>();
        public decimal Subtotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal DescuentoAplicado { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
        public string CodigoCupon { get; set; }
    }
}
