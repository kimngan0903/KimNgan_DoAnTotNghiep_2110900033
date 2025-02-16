using Microsoft.AspNetCore.Mvc;

namespace PlantCare.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
