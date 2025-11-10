using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.AccountModels
{
    public class RecoveryCodeLoginDTO
    {
        [Required(ErrorMessage = "Recovery code is required")]
        [Display(Name = "Recovery Code", Prompt = "Enter your recovery code")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "Recovery code must be between 8 and 10 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Recovery code can only contain letters and numbers")]
        public string RecoveryCode { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        public int RemainingCodes { get; set; }

        public string MaskedEmail { get; set; } = string.Empty;

        public bool ShowRegenerateWarning => RemainingCodes <= 2;

        public string WarningMessage => RemainingCodes switch
        {
            0 => "This is your last recovery code. Generate new codes after logging in.",
            1 => "You have only 1 recovery code remaining after this one.",
            2 => "You have only 2 recovery codes remaining after this one.",
            _ => string.Empty
        };
    }
}
