using System.ComponentModel.DataAnnotations;

namespace ProgramASP.ViewModels.AccountModels
{
    public class RegisterViewModel
    {
        [Phone]
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        public bool? Rights { get; set; }
    }
}
