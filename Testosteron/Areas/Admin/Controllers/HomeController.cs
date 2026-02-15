using Microsoft.AspNetCore.Mvc;

namespace Testosteron.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddTest()
        {
            //создать тест в базе данных
            //redirect на редактирование теста
            return Ok();
        }

        public IActionResult EditTest(Guid guid)
        {
            // redirect
            return Ok();
        }

        public IActionResult DeleteTest(Guid guid)
        {
            // update view
            return Ok();
        }
    }
}
