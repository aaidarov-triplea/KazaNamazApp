using System.ComponentModel.DataAnnotations;

namespace KN.WEB.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password), Display(Name = "Пароль")]
    public string Password { get; set; } = "";

    [Display(Name = "Запомнить меня")]
    public bool RememberMe { get; set; }
}