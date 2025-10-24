using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace eCommerce.Entities.ViewModels
{
    public class ArmaPcViewModel
    {
        // Información del paso actual
        public int Paso { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        // Marca seleccionada (AMD o Intel)
        public string MarcaSeleccionada { get; set; }

        // IDs de componentes seleccionados
        public int? IdProcesadorSeleccionado { get; set; }
        public int? IdMotherboardSeleccionado { get; set; }
        public List<int> IdsRamSeleccionados { get; set; } = new List<int>();
        public int? IdGpuSeleccionada { get; set; }
        public List<int> IdsAlmacenamientoSeleccionados { get; set; } = new List<int>();
        public int? IdPsuSeleccionada { get; set; }
        public int? IdGabineteSeleccionado { get; set; } 

        // Totales
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int TotalComponentes { get; set; }

        // Listas de productos disponibles por categoría
        public List<ProductoDTO> ProcesadoresDisponibles { get; set; } = new List<ProductoDTO>();
        public List<ProductoDTO> MotherboardsDisponibles { get; set; } = new List<ProductoDTO>();
        public List<ProductoDTO> RamsDisponibles { get; set; } = new List<ProductoDTO>();
        public List<ProductoDTO> GpusDisponibles { get; set; } = new List<ProductoDTO>();
        public List<ProductoDTO> AlmacenamientoDisponible { get; set; } = new List<ProductoDTO>();
        public List<ProductoDTO> PsusDisponibles { get; set; } = new List<ProductoDTO>();
        public List<ProductoDTO> GabinetesDisponibles { get; set; } = new List<ProductoDTO>(); // ⭐ GABINETES

        // Información de componentes seleccionados
        public ComponenteSeleccionadoDTO ProcesadorSeleccionadoInfo { get; set; }
        public ComponenteSeleccionadoDTO MotherboardSeleccionadoInfo { get; set; }
        public List<ComponenteSeleccionadoDTO> ComponentesSeleccionados { get; set; } = new List<ComponenteSeleccionadoDTO>();

        // Propiedad helper
        public bool TieneGabineteSeleccionado => IdGabineteSeleccionado.HasValue && IdGabineteSeleccionado.Value > 0;
    }

    // DTO de Producto
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string RutaImagen { get; set; }
        public string NombreImagen { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public string CategoriaPadre { get; set; }
        public List<EspecificacionProductoDTO> Especificaciones { get; set; } = new List<EspecificacionProductoDTO>();

        // Propiedades calculadas
        public bool EnStock => Stock > 0;
        public string ResumenEspecificaciones => string.Join(" | ", Especificaciones.Take(3).Select(e => $"{e.Clave}: {e.Valor}"));
    }

    // DTO de Especificación
    public class EspecificacionProductoDTO
    {
        public string Clave { get; set; }
        public string Valor { get; set; }
    }

    // DTO de Componente Seleccionado
    public class ComponenteSeleccionadoDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; } // Procesador, Motherboard, RAM, GPU, etc.
        public string Marca { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
        public string RutaImagen { get; set; }
    }

    // ViewModel para guardar configuración
    public class GuardarConfiguracionViewModel
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public ArmaPcViewModel Configuracion { get; set; }
    }

    // DTO de Configuración Guardada
    public class ConfiguracionGuardadaDTO
    {
        public int IdConfiguracion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int TotalComponentes { get; set; }
        public List<ComponenteSeleccionadoDTO> Componentes { get; set; } = new List<ComponenteSeleccionadoDTO>();
    }

    // ViewModel auxiliar para pasos
    public class ArmaPcPasoViewModel
    {
        public int Paso { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public List<ProductoDTO> ProductosDisponibles { get; set; } = new List<ProductoDTO>();
        public int? ProductoSeleccionadoId { get; set; }
        public string MarcaActual { get; set; }
        public ArmaPcViewModel ConfiguracionActual { get; set; }
    }
}