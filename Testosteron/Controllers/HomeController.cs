using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Testosteron.Models;

namespace Testosteron.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet("/")]
        [HttpGet("home")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
