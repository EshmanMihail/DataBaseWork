using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class EnterpriseController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;

        public EnterpriseController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
        }

        [ResponseCache(Duration = CacheDuration)]
        public IActionResult ShowTable()
        {
            ViewBag.data = db.Enterprises.Take(30).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Update(int id, string name, string organization)
        {

            ViewBag.data = db.Enterprises.Take(30).ToList();
            return View("ShowTable");
        }
    }
}
