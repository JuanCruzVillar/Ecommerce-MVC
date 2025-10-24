using eCommerce.Entities;
using eCommerce.Entities.ViewModels;
using eCommerce.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Services.Interfaces
{
    public interface IArmatuPcService
    {
        Task<List<ProductoDTO>> ObtenerProcesadoresAsync(string marca);
        Task<List<ProductoDTO>> ObtenerMotherboardsAsync(string marca);
        Task<List<ProductoDTO>> ObtenerRamsAsync();
        Task<List<ProductoDTO>> ObtenerGpusAsync();
        Task<List<ProductoDTO>> ObtenerAlmacenamientoAsync();
        Task<List<ProductoDTO>> ObtenerPsusAsync();
        Task<List<ProductoDTO>> ObtenerCoolersAsync();

        Task<List<ProductoDTO>> ObtenerGabinetesAsync();
        Task<ProductoDTO> ObtenerProductoDetalladoAsync(int idProducto);
        Task<ArmaPcViewModel> ObtenerConfiguracionActualAsync(int idCliente);
        Task<bool> GuardarConfiguracionAsync(int idCliente, GuardarConfiguracionViewModel model);
        Task<List<ConfiguracionGuardadaDTO>> ObtenerConfiguracionesGuardasAsync(int idCliente);
        Task<ConfiguracionGuardadaDTO> ObtenerConfiguracionGuardadaAsync(int idConfiguracion, int idCliente);
        Task<bool> EliminarConfiguracionAsync(int idConfiguracion, int idCliente);
        Task<decimal> CalcularTotalAsync(ArmaPcViewModel configuracion);
    }
}