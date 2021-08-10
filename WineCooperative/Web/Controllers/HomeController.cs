using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index() => View();

        //ToDo:
        public IActionResult Privacy() => View();

        //ToDo
        public IActionResult AboutUs() => View();

        //toDo:
        public IActionResult Contacts() => View(); 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
