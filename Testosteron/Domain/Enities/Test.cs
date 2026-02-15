using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Testosteron.Domain.Enities;

public class Test
{
    [Required]
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(600)]
    public string Description { get; set; } = string.Empty;

    [Required, Column(TypeName = "jsonb")]
    public List<TestField> TestFields { get; set; } = new();
}

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
