using E_Commerce.BLL.Models.Profile;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ProfileModels
{
    public class RecoveryDTO
    {
        [Required(ErrorMessage = "Id Not Passed")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } 

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }
        public int CountDown { get; set; } 

        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must contain only numbers")]
        public string OtpCode { get; set; }

        [Required(ErrorMessage = "ReturnUrl is required")]
        public string? ReturnUrl { get; set; }
        public int Step { get; set; } = 1;
        public bool IsReset { get; set; } = false;
        public string FormName { get; set; } = "email";

        public ConcurrentDictionary<int, string> Questions = new ConcurrentDictionary<int, string>(
            new[]{
             new KeyValuePair<int, string>(0, "What was the name of your first pet?"),
             new KeyValuePair<int, string>(1, "What was the name of your elementary school?"),
             new KeyValuePair<int, string>(2, "In what city were you born?"),
             new KeyValuePair<int, string>(3, "What is your mother's maiden name?"),
             new KeyValuePair<int, string>(4, "What was the make of your first car?"),
             new KeyValuePair<int, string>(5, "What street did you grow up on?"),
             new KeyValuePair<int, string>(6, "What is your favorite book?"),
             new KeyValuePair<int, string>(7, "What was your favorite teacher's name?")});
        [Required]
        [MinLength(3, ErrorMessage = "At least 3 items are required.")]
        public List<QA>? Answers { get; set; } = new List<QA>() {
            new QA(),new QA(),new QA()
        };
        public ValidationResultDTO? otpObject { get; set; } = new ValidationResultDTO();
        public string? PassDictionaryJson { get; set; }
        public string? RecovEmail { get; set; }
        public string? RecovPhone { get; set; }
    }
    public class QA
    {
        [RegularExpression("^(?!Select a question...$).*$", ErrorMessage = "Value cannot be 'Select a question...'.")]
        public string Question { get; set; } 
        [Required(ErrorMessage ="Please enter the answer")]
        public string Answer { get; set; }

    }
}
