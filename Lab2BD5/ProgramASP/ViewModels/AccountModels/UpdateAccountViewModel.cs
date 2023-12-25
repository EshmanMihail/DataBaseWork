using System.ComponentModel.DataAnnotations;

namespace ProgramASP.ViewModels.AccountModels
{
    public class UpdateAccountViewModel
    {
        [Required]
        public string? OldPhoneNumber { get; set; }

        public string? NewPhoneNumber { get; set; }

        public bool? NewRights { get; set; }
    }
}
