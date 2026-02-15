using Testosteron.Domain.Enities;

public class TestViewModel
{
    public Guid TestId { get; set; } = Guid.Empty;
    public string TestTitle { get; set; } = string.Empty;
    public string TestDescription { get; set; } = string.Empty;
    public IEnumerable<TestField> TestFields { get; set; } = new List<TestField>();

    public List<FieldAnswer> Answers { get; set; } = new();

    public TestViewModel(Test test)
    {
        TestId = test.Id;
        TestTitle = test.Title;
        TestDescription = test.Description;
        TestFields = test.TestFields;

        foreach (var field in test.TestFields)
        {
            var fieldType = field.TestFieldType?.ToLower() ?? "text";
            var options = field.Options ?? Array.Empty<string>();

            var answer = new FieldAnswer
            {
                FieldType = fieldType,
                Options = options
            };

            // Инициализируем списки в зависимости от типа
            if (fieldType == "check")
            {
                // Создаем список булевых значений для каждой опции
                answer.CheckboxValues = new List<bool>();
                for (int j = 0; j < options.Length; j++)
                {
                    answer.CheckboxValues.Add(false);
                }
            }
            else if (fieldType == "radio")
            {
                answer.RadioIndex = null;
            }
            else
            {
                answer.TextValue = string.Empty;
            }

            Answers.Add(answer);
        }
    }

    public TestViewModel() { }
}