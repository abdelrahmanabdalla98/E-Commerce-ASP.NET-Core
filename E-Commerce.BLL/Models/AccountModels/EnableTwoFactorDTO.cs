using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.AccountModels
{
    public class EnableTwoFactorDTO
    {
        [Required(ErrorMessage = "Verification code is required")]
        [StringLength(7, ErrorMessage = "Code must be 6 digits", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;

        public string SharedKey { get; set; } = string.Empty;
        public string AuthenticatorUri { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }

        // Enhanced properties for better 2FA setup experience
        //[Display(Name = "Authenticator App")]
        //public string AuthenticatorApp { get; set; } = "Google Authenticator";

        //public bool HasExistingRecoveryCodes { get; set; }

        //public int ExistingRecoveryCodesCount { get; set; }

        //public DateTime? LastRecoveryCodesGenerated { get; set; }

        //public bool IsReEnabling { get; set; }

        //public string? BackupEmail { get; set; }

        //public bool SendBackupCodesToEmail { get; set; }

        //// Step tracking for multi-step setup process
        //public int CurrentStep { get; set; } = 1;
        //public int TotalSteps { get; set; } = 3;

        //// Helper properties
        //public string FormattedSharedKey => SharedKey != null ?
        //    string.Join(" ", Enumerable.Range(0, SharedKey.Length / 4)
        //        .Select(i => SharedKey.Substring(i * 4, Math.Min(4, SharedKey.Length - i * 4)))) : string.Empty;

        //public string StepDescription => CurrentStep switch
        //{
        //    1 => "Install an authenticator app on your mobile device",
        //    2 => "Add your account to the authenticator app",
        //    3 => "Verify the setup with a code from your app",
        //    _ => "Complete two-factor authentication setup"
        //};

        //public double ProgressPercentage => ((double)CurrentStep / TotalSteps) * 100;

        //public bool IsSetupComplete => CurrentStep >= TotalSteps && RecoveryCodes?.Length > 0;

        //public List<string> RecommendedApps { get; set; } = new()
        //{
        //    "Google Authenticator",
        //    "Microsoft Authenticator",
        //    "Authy",
        //    "1Password",
        //    "Bitwarden Authenticator"
        //};
    }
}
