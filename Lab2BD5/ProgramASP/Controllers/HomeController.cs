using Microsoft.AspNetCore.Mvc;

namespace ProgramASP.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "hello world!";
        }
    }
}
