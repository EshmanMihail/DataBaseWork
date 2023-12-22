using InfoStruct.Service;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class HomeController : Controller
    {
        private HeatSchemeStorageContext context;
        public HomeController([FromServices] HeatSchemeStorageContext context) 
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
