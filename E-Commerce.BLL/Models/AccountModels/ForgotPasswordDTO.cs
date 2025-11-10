using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.AccountModels
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address", Prompt = "Enter your email address")]
        [StringLength(256, ErrorMessage = "Email address cannot exceed 256 characters")]
        public string Email { get; set; } = string.Empty;   

        //public bool EmailSent { get; set; }

        //public DateTime? EmailSentAt { get; set; }

        //public int ResendCooldownMinutes { get; set; } = 5;

        //public bool CanResend => !EmailSentAt.HasValue ||
        //    DateTime.UtcNow > EmailSentAt.Value.AddMinutes(ResendCooldownMinutes);

        //public string TimeUntilResend
        //{
        //    get
        //    {
        //        if (CanResend) return string.Empty;

        //        var timeRemaining = EmailSentAt.Value.AddMinutes(ResendCooldownMinutes) - DateTime.UtcNow;
        //        return $"{(int)timeRemaining.TotalMinutes}:{timeRemaining.Seconds:D2}";
        //    }
        //}
    }
}
