using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Testosteron.Domain;
using Testosteron.Domain.Enities;
using Testosteron.Domain.Repositories;
using Testosteron.Services;

namespace Testosteron.Controllers
{
    [Route("Test")]
    public class TestController : Controller
    {
        private readonly TestManager _testManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestController(TestManager testManager, UserManager<ApplicationUser> userManager)
        {
            _testManager = testManager;
            _userManager = userManager;
        }

        //[X] Сделать View
        //[X] Вывести тест по guid
        // - разрешить не авторизованных пользователей
        [HttpGet]
        public async Task<IActionResult> Test(Guid guid)
        {
            var result = await _testManager.GetTestByIdAsync(guid);

            if (!result.Success)
            {
                return this.NotFound($"Страница Test/{guid.ToString()} не найдена");
            }

            TestViewModel model = new TestViewModel(result.Value!);

            return View(model);
        }

        [HttpPost("{guid}")]
        public async Task<IActionResult> Test(Guid guid, [FromForm] TestViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            Guid? userId = currentUser == null ? null : currentUser.Id;

            if (userId.HasValue)
            {
                var answers = await _testManager.GetUserAnswersForTest(userId.Value, viewModel.TestId);

                if (answers.Success)
                {
                    return Content("вы уже дали ответ на этот тест");
                }
            }


            var result = await _testManager.AddAnswersToTest(new() { TestId = viewModel.TestId, UserId = userId });

            return Json(result);
        }
    }
}
