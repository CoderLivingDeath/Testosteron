using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Testosteron.Domain.Enities;

public class Answers
{
    [Key] public Guid Id { get; set; }

    [Required]
    public Guid TestId { get; set; }

    public Guid? UserId { get; set; }

    [Required]
    [Column("ContentJson", TypeName = "jsonb")]
    [MaxLength(4000)]
    public AnswersContainer Content { get; set; } = new();

    [ForeignKey("TestId")]
    public Test Test { get; set; } = null!;

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; } = null!;
}

public class AnswersContainer
{
    public List<FieldAnswer> Content { get; set; } = new();

    public AnswersContainer()
    {
        
    }

    public AnswersContainer(List<FieldAnswer> content)
    {
        Content = content;
    }
}
