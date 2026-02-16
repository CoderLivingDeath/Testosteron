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
