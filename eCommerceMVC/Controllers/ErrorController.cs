using Microsoft.AspNetCore.Mvc;

namespace eCommerceMVC.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        // Ruta: /Error o /Error/500
        [Route("Error")]
        [Route("Error/{statusCode}")]
        public IActionResult Index(int? statusCode = null)
        {
            var statusCodeInt = statusCode ?? Response.StatusCode;

            // Detectar si el error vino de Admin o Cliente
            var referer = Request.Headers["Referer"].ToString();
            var isAdminArea = referer.Contains("/Admin", StringComparison.OrdinalIgnoreCase);

            ViewBag.StatusCode = statusCodeInt;
            ViewBag.IsAdminArea = isAdminArea;
            ViewBag.ErrorMessage = GetErrorMessage(statusCodeInt);
            ViewBag.ErrorDescription = GetErrorDescription(statusCodeInt);

            _logger.LogWarning(
                "Error {StatusCode} mostrado. Area: {Area}, Path: {Path}, Referer: {Referer}",
                statusCodeInt,
                isAdminArea ? "Admin" : "Cliente",
                HttpContext.Request.Path,
                referer
            );

            return View();
        }

        // Ruta específica para acceso denegado
        [Route("Error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            var referer = Request.Headers["Referer"].ToString();
            var isAdminArea = referer.Contains("/Admin", StringComparison.OrdinalIgnoreCase);

            ViewBag.IsAdminArea = isAdminArea;

            return View();
        }

        #region Helper Methods

        private string GetErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Solicitud incorrecta",
                401 => "No autorizado",
                403 => "Acceso denegado",
                404 => "Página no encontrada",
                500 => "Error interno del servidor",
                503 => "Servicio no disponible",
                _ => "Ha ocurrido un error"
            };
        }

        private string GetErrorDescription(int statusCode)
        {
            return statusCode switch
            {
                400 => "La solicitud no pudo ser procesada. Verifica los datos ingresados.",
                401 => "Debes iniciar sesión para acceder a este recurso.",
                403 => "No tienes permisos para acceder a este recurso.",
                404 => "La página que buscas no existe o fue movida.",
                500 => "Algo salió mal en nuestro servidor. Estamos trabajando para solucionarlo.",
                503 => "El servicio está temporalmente no disponible. Intenta más tarde.",
                _ => "Por favor, intenta nuevamente más tarde."
            };
        }

        #endregion
    }
}