using System.ComponentModel.DataAnnotations;

namespace ProgramASP.ViewModels.AccountModels
{
    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
