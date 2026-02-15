public class FieldAnswer
{
    public Guid FieldId { get; set; }
    public string FieldType { get; set; } = "text";
    public string[] Options { get; set; } = Array.Empty<string>();

    // Для чекбоксов: список bool для каждой опции
    public List<bool>? CheckboxValues { get; set; }

    // Для радиокнопок: индекс выбранной опции
    public int? RadioIndex { get; set; }

    // Для текстовых полей: текст ответа
    public string? TextValue { get; set; }

    // Вспомогательные методы
    public bool IsCheckbox => FieldType == "check";
    public bool IsRadio => FieldType == "radio";
    public bool IsText => FieldType == "text" || string.IsNullOrEmpty(FieldType);

    // Конвертация CheckboxValues в SelectedIndices
    public List<int> GetSelectedCheckboxIndices()
    {
        if (CheckboxValues == null) return new List<int>();

        var indices = new List<int>();
        for (int i = 0; i < CheckboxValues.Count; i++)
        {
            if (CheckboxValues[i]) indices.Add(i);
        }
        return indices;
    }

    // Установка CheckboxValues из индексов
    public void SetCheckboxIndices(List<int> indices)
    {
        if (CheckboxValues == null) return;

        // Сначала все false
        for (int i = 0; i < CheckboxValues.Count; i++)
        {
            CheckboxValues[i] = false;
        }

        // Потом отмечаем выбранные
        foreach (var index in indices)
        {
            if (index < CheckboxValues.Count)
            {
                CheckboxValues[index] = true;
            }
        }
    }
}