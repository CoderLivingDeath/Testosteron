using System.Text.Json;
using System.Text.Json.Serialization;

namespace Testosteron.Services
{
    public class ResultJsonConverter : JsonConverter<Result>
    {
        public override Result Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var success = root.TryGetProperty("success", out var successEl) && successEl.GetBoolean();
            var message = root.TryGetProperty("message", out var msg) && msg.ValueKind != JsonValueKind.Null
                ? msg.GetString() ?? ""
                : "";
            string[]? errors = null;
            if (root.TryGetProperty("errors", out var errs) && errs.ValueKind == JsonValueKind.Array)
            {
                errors = JsonSerializer.Deserialize<string[]>(errs.GetRawText(), options);
            }

            return new Result(success, errors, message);
        }

        public override void Write(Utf8JsonWriter writer, Result value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("success", value.Success);
            writer.WriteString("message", value.Message);
            if (value.Errors != null && value.Errors.Length > 0)
            {
                writer.WritePropertyName("errors");
                JsonSerializer.Serialize(writer, value.Errors, options);
            }
            writer.WriteEndObject();
        }
    }

    public class ResultTJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType &&
                   typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var genericType = typeToConvert.GetGenericArguments()[0];
            var converterType = typeof(ResultTJsonConverter<>).MakeGenericType(genericType);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }

    public class ResultTJsonConverter<T> : JsonConverter<Result<T>>
    {
        public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var success = root.TryGetProperty("success", out var successEl) && successEl.GetBoolean();
            var message = root.TryGetProperty("message", out var msg) && msg.ValueKind != JsonValueKind.Null
                ? msg.GetString() ?? ""
                : "";
            string[]? errors = null;
            if (root.TryGetProperty("errors", out var errs) && errs.ValueKind == JsonValueKind.Array)
            {
                errors = JsonSerializer.Deserialize<string[]>(errs.GetRawText(), options);
            }

            T value = default!;
            if (root.TryGetProperty("value", out var val) && val.ValueKind != JsonValueKind.Null)
            {
                value = JsonSerializer.Deserialize<T>(val.GetRawText(), options)!;
            }

            return new Result<T>(value, success, errors, message);
        }

        public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("success", value.Success);
            writer.WriteString("message", value.Message);

            writer.WritePropertyName("value");
            JsonSerializer.Serialize(writer, value.Value, options);

            if (value.Errors != null && value.Errors.Length > 0)
            {
                writer.WritePropertyName("errors");
                JsonSerializer.Serialize(writer, value.Errors, options);
            }
            writer.WriteEndObject();
        }
    }
}
