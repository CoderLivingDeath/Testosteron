using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Testosteron.Models.Validation;

namespace Testosteron.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Логин обязателен")]
    [EmailAddress(ErrorMessage = "Логин должен быть в формате email (например: user@domain.com)")]
    [MinLength(6, ErrorMessage = "Логин должен содержать минимум 6 символов")]
    [RegularExpression(@"^[a-zA-Z0-9@.]+$", ErrorMessage = "Только латинские буквы, цифры и символы @ .")]
    [DisplayName("Логин")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [PasswordComplexity]
    [DisplayName("Пароль")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтверждение пароля обязательно")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DisplayName("Подтверждение пароля")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
