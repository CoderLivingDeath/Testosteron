using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Testosteron.Models.Validation;

public class PasswordComplexityAttribute : ValidationAttribute, IClientModelValidator
{
    public int MinLength { get; set; } = 3;

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-passwordcomplexity", ErrorMessage ?? "Пароль не соответствует требованиям");
        MergeAttribute(context.Attributes, "data-val-passwordcomplexity-minlength", MinLength.ToString());
    }

    private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (attributes.ContainsKey(key))
            return false;
        attributes.Add(key, value);
        return true;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Пароль обязателен");
            }

            if (password.Length < MinLength)
            {
                return new ValidationResult($"Пароль должен содержать минимум {MinLength} символов");
            }

            if (!password.Any(char.IsUpper))
            {
                return new ValidationResult("Пароль должен содержать хотя бы одну заглавную букву");
            }

            if (!password.Any(char.IsLower))
            {
                return new ValidationResult("Пароль должен содержать хотя бы одну строчную букву");
            }

            var latinOnlyRegex = new Regex(@"^[a-zA-Z0-9]+$");
            if (!latinOnlyRegex.IsMatch(password))
            {
                return new ValidationResult("Только латинские буквы и цифры");
            }
        }
        return ValidationResult.Success;
    }
}
