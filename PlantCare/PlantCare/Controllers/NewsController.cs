using Microsoft.AspNetCore.Mvc;

namespace PlantCare.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
