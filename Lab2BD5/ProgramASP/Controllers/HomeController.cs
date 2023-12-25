using InfoStruct.Service;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class HomeController : Controller
    {
        private HeatSchemeStorageContext context;
        private List<string> list = new List<string>() { "huy", "zalupa", "penis", "chlen" };
        public HomeController([FromServices] HeatSchemeStorageContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
