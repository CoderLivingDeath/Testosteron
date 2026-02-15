using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Testosteron.Models
{
    public class LoginModel
    {
        [Required, DisplayName("Логин")]
        public string UserName { get; set; }

        [Required, DisplayName("Пароль")]
        public string Password { get; set; }

        [Required, DisplayName("Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
