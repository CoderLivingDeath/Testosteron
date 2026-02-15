using Newtonsoft.Json;
using Testosteron.Domain.Enities;

namespace Testosteron.Areas.Admin.Models
{
    public class EditTestViewModel
    {
        public Guid Guid { get; set; }

        public string TestTitle { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<AdminTestFieldViewModel> Fields { get; set; } = new();

        public EditTestViewModel()
        {
            Guid = Guid.Empty;
            TestTitle = "Test title default";
        }

        public EditTestViewModel(Test test)
        {
            Guid = test.Id;
            TestTitle = test.Title;
            Description = test.Description;

            Fields = test.TestFields
                .Select((field, index) => new AdminTestFieldViewModel(field, index))
                .ToList();
        }
    }

    public class AdminTestFieldViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;
        public string TestFieldType { get; set; } = "text";
        public bool Required { get; set; }
        public string[] Options { get; set; } = Array.Empty<string>();

        public string[] AvailableTypes { get; set; } = { "text", "check", "radio" };

        public AdminTestFieldViewModel()
        {
            
        }

        public AdminTestFieldViewModel(TestField field, int index)
        {
            Title = field.Title;
            Description = field.Description;
            TestFieldType = field.TestFieldType;
            Required = field.Required;
            Options = field?.Options ?? Array.Empty<string>();

        }

        public TestField ToTestField()
        {
            return new()
            {
                Title = Title,
                Description = Description ?? string.Empty,
                Required = Required,
                Options = Options,
                TestFieldType = TestFieldType
            };
        }

    }

}
