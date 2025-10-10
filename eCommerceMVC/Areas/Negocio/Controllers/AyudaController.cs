using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    
    [Area("Negocio")]
    public class AyudaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }


}