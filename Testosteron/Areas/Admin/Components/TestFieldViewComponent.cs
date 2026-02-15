using Microsoft.AspNetCore.Mvc;
using Testosteron.Areas.Admin.Models;

namespace Testosteron.Areas.Admin.Components
{
    public class TestFieldViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string index)
        {
            var vm = new AdminTestFieldViewModel
            {
                Title = "Новое поле",
                Description = "",
                TestFieldType = "text",
                Required = false,
                Options = Array.Empty<string>()
            };

            ViewData.TemplateInfo.HtmlFieldPrefix = $"Fields[{index}]";

            return View("~/Areas/Admin/Views/Shared/_FieldItem.cshtml", vm);
        }
    }
}
