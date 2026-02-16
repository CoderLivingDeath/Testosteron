using System.ComponentModel.DataAnnotations;

namespace Testosteron.Domain.Enities;

public class TestField
{
    [Required]
    [Length(minimumLength: 0, maximumLength: 100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string TestFieldType { get; set; } = string.Empty;

    [Required]
    public bool Required { get; set; }

    [Required]
    [Length(minimumLength: 0, maximumLength: 400)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string[] Options { get; set; } = Array.Empty<string>();
}
