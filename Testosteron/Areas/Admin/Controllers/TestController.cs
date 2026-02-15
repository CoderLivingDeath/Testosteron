using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Testosteron.Areas.Admin.Models;
using Testosteron.Data;
using Testosteron.Domain.Enities;
using Testosteron.Services;

namespace Testosteron.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TestController : Controller
    {
        private readonly IRepository<Test> _testRepository;
        private readonly TestManager _testManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public TestController(IRepository<Test> testRepository, ApplicationDbContext applicationDbContext, TestManager testManager)
        {
            _testRepository = testRepository;
            this._applicationDbContext = applicationDbContext;
            _testManager = testManager;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var newTestResult = await _testManager.AddNewTestAsync(new() { Description = "New Test", TestFields = [], Title = "New Test" });

            return Json(newTestResult);
        }

        public async Task<IActionResult> Edit(Guid guid)
        {
            var test = await _testManager.GetTestByIdAsync(guid);

            if (!test.Success) return NotFound(new { success = false, message = test.Message, errors = test.Message });
            else
            {
                EditTestViewModel vm = new EditTestViewModel(test.Value!);
                return View("EditTest", vm);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var result = await _testManager.DeleteTestAsync(new() { Id = guid });

            return Json(result);
        }

        [HttpPatch]
        [Area("Admin")]
        public async Task<IActionResult> Edit(Guid guid, [FromForm] EditTestViewModel model)
        {
            UpdateTestDTO update = new()
            {
                Id = guid,
                Description = model.Description,
                TestFields = model.Fields.Select(item => item.ToTestField()).ToList(),
                Title = model.TestTitle
            };

            var result = await _testManager.UpdateTestAsync(update);

            return Json(result);
        }

        public IActionResult GetFieldTemplate(string index)
        {
            return ViewComponent("TestField", new { index });
        }

        public IActionResult Preview(Guid guid)
        {
            return View();
        }
    }
}

public static class HttpRequestExtensions
{
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest";
    }
}
