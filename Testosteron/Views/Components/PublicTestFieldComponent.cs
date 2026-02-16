using Microsoft.AspNetCore.Mvc;
using Testosteron.Domain.Enities;

namespace Testosteron.Views.Components;

[ViewComponent(Name = "PublicTestField")]
public class PublicTestFieldComponent : ViewComponent
{
    public IViewComponentResult Invoke(string prefix, TestFieldComponentViewModel field)
    {
        ViewData.TemplateInfo.HtmlFieldPrefix = prefix;

        return View(field);
    }
}

public class TestFieldComponentViewModel
{
    public TestField Field { get; set; } = new();

    public bool IsCheck => Field.TestFieldType == "check";
    public bool IsRadio => Field.TestFieldType == "radio";
    public bool IsText => Field.TestFieldType == "text";

    public bool[]? CheckboxValues { get; set; }
    public int? RadioIndex { get; set; }
    public string[]? TextValues { get; set; }

    public TestFieldComponentViewModel()
    {
        
    }

    public TestFieldComponentViewModel(TestField field)
    {
        Field = field;
    }

    public FieldAnswer GetAnswer()
    {
        return new()
        {
            FieldType = Field.TestFieldType,
            CheckboxValues = CheckboxValues,
            RadioIndex = RadioIndex,
            TextValue = TextValues
        };
    }
}
