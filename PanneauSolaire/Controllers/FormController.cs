using Microsoft.AspNetCore.Mvc;

namespace PanneauSolaire.Controllers
{
    public class FormController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
