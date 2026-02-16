using Testosteron.Domain.Enities;
using Testosteron.Views.Components;

public class TestViewModel
{
    public Guid TestId { get; set; } = Guid.Empty;
    public string TestTitle { get; set; } = string.Empty;
    public string TestDescription { get; set; } = string.Empty;
    public IEnumerable<TestFieldComponentViewModel> TestFields { get; set; } = new List<TestFieldComponentViewModel>();

    public TestViewModel(Test test)
    {
        TestId = test.Id;
        TestTitle = test.Title;
        TestDescription = test.Description;
        TestFields = test.TestFields.Select(item => new TestFieldComponentViewModel(item));
    }

    public TestViewModel() { }
}