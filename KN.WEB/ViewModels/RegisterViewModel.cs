using System.ComponentModel.DataAnnotations;

namespace KN.WEB.ViewModels;

public class RegisterViewModel
{
    [Required, Display(Name = "Имя")] 
    public string FullName { get; set; } = "";
    [Required, EmailAddress]
    public string Email { get; set; } = "";
    [Required, DataType(DataType.Password), Display(Name = "Пароль")]
    public string Password { get; set; } = "";
    [DataType(DataType.Password), Display(Name = "Подтверждение пароля")]
    [Compare(nameof(Password), ErrorMessage = "Пароль не совпадают")]
    public string ConfirmPassword { get; set; } = "";
}