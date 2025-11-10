using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.AccountModels
{
    public class TwoFactorDTO
    {
        [Required(ErrorMessage = "Verification code is required")]
        [StringLength(7, ErrorMessage = "Code must be 6 digits", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
