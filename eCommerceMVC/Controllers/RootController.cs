using Microsoft.AspNetCore.Mvc;

public class RootController : Controller
{
    public IActionResult Index()
    {
        
        return RedirectToAction("Index", "Home", new { area = "Negocio" });
    }
}
