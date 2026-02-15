using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Testosteron.Services
{


    public interface IResult
    {
        public bool Success { get; }
        string[]? Errors { get; }
        string Message { get; }
    }

    public interface IResult<T> : IResult
    {
        T Value { get; }
    }

    public struct Result : IResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Errors { get; set; }

        [JsonConstructor]
        public Result(bool success , string[]? errors, string message)
        {
            Success = success;
            Errors = errors;
            Message = message;
        }

        public Result ErrorsFromException(Exception exception)
        {
            var errorMessages = new List<string>();
            if (exception.Message is not null)
                errorMessages.Add(exception.Message);
            if (exception.InnerException?.Message is { } innerMessage)
                errorMessages.Add(innerMessage);

            Errors = Errors == null 
                ? errorMessages.ToArray() 
                : Errors.Concat(errorMessages).ToArray();

            return this;
        }

        public static Result CreateSuccess(string message)
        {
            return new Result(true, null, message);
        }

        public static Result CreateFailure(string message = "", string[]? errors = null)
        {
            return new Result(false, errors ?? Array.Empty<string>(), message);
        }
    }

    public struct Result<T> : IResult<T>
    {
        public bool Success { get; set; }

        [JsonPropertyOrder(4)]
        public T Value { get; set; }

        [JsonPropertyOrder(6)]
        public string Message { get; set; }

        [JsonPropertyOrder(7)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Errors { get; set; }

        [JsonConstructor]
        public Result(T value, bool success, string[]? errors, string message)
        {
            Success = success;
            Value = value;
            Errors = errors;
            Message = message;
        }
        public Result<T> ErrorsFromException(Exception exception)
        {
            var errorMessages = new List<string>();
            if (exception.Message is not null)
                errorMessages.Add(exception.Message);
            if (exception.InnerException?.Message is { } innerMessage)
                errorMessages.Add(innerMessage);

            Errors = Errors == null
                ? errorMessages.ToArray()
                : Errors.Concat(errorMessages).ToArray();

            return this;
        }

        public static Result<T> CreateSuccess(T value, string message = "")
        {
            return new Result<T>(value, true, null, message);
        }

        public static Result<T> CreateFailure(T value, string message = "", string[]? errors = null)
        {
            return new Result<T>(value, false, errors ?? Array.Empty<string>(), message);
        }
    }
}
