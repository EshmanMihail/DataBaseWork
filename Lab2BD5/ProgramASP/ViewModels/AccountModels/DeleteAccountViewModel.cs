using System.ComponentModel.DataAnnotations;

namespace ProgramASP.ViewModels.AccountModels
{
    public class DeleteAccountViewModel
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}
