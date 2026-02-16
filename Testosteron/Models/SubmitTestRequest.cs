namespace Testosteron.Models
{
    public class SubmitTestRequest
    {
        public Guid testId { get; set; }
        public List<FieldAnswer> answers { get; set; } = new();
    }
}
