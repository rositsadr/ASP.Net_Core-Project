using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class MembersController : Controller
    {
        public IActionResult All() => View();

        public IActionResult Add() => View();

        public IActionResult DisplayMember() => View();
    }
}
