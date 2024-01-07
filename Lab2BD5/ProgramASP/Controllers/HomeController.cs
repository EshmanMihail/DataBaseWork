using InfoStruct.Service;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class HomeController : Controller
    {
        private HeatSchemeStorageContext context;
        private List<string> list = new List<string>() { };
        public HomeController([FromServices] HeatSchemeStorageContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string language)
        {
            if (language != null)
            {
                list.Add(language);
            }
            return RedirectToAction("About");
        }

        public IActionResult About()
        {
            ViewBag.List = list;
            return View();
        }
    }
}
