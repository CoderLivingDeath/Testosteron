using System.Text.Json.Serialization;

public class FieldAnswer
{
    public string FieldType { get; set; } = "text";

    // Для чекбоксов: список bool для каждой опции
    public bool[]? CheckboxValues { get; set; }

    // Для радиокнопок: индекс выбранной опции
    public int? RadioIndex { get; set; }

    // Для текстовых полей: текст ответа
    public string[]? TextValue { get; set; }

    // Вспомогательные методы
    [JsonIgnore]
    public bool IsCheckbox => FieldType == "check";
    [JsonIgnore]
    public bool IsRadio => FieldType == "radio";
    [JsonIgnore]
    public bool IsText => FieldType == "text" || string.IsNullOrEmpty(FieldType);

    // Конвертация CheckboxValues в SelectedIndices
    public List<int> GetSelectedCheckboxIndices()
    {
        if (CheckboxValues == null) return new List<int>();

        var indices = new List<int>();
        for (int i = 0; i < CheckboxValues.Length; i++)
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
        for (int i = 0; i < CheckboxValues.Length; i++)
        {
            CheckboxValues[i] = false;
        }

        // Потом отмечаем выбранные
        foreach (var index in indices)
        {
            if (index < CheckboxValues.Length)
            {
                CheckboxValues[index] = true;
            }
        }
    }
}