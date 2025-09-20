using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Services.Interfaces;



namespace eCommerce.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutViewModel> ObtenerCheckoutAsync(int idCliente);
        Task<CuponResultViewModel> ValidarCuponAsync(string codigoCupon, decimal subtotal);
        Task<bool> AgregarDireccionEnvioAsync(DireccionEnvioViewModel direccionVM, int idCliente);
        Task<decimal> CalcularCostoEnvioAsync(int idDireccionEnvio);
        Task<int> ProcesarPedidoAsync(CheckoutViewModel checkoutVM, int idCliente);
        Task<PedidoConfirmacionViewModel> ObtenerConfirmacionPedidoAsync(int idVenta);
        Task<bool> ValidarStockProductosAsync(int idCliente);
    }
}
