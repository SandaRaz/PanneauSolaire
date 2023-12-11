using Microsoft.AspNetCore.Mvc;

namespace PanneauSolaire.Controllers
{
    public class SpecificController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Coupure()
        {
            return View();
        }
    }
}
