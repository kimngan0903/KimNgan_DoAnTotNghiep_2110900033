using Microsoft.AspNetCore.Mvc;

namespace PlantCare.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
